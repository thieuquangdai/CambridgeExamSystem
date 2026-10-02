(function ($) {
    'use strict';

    const config = JSON.parse(document.getElementById('exam-config').textContent);
    const storageKey = 'cambridge-attempt-' + config.examAttemptId;
    const HEARTBEAT_MS = 30000;
    const DEBOUNCE_MS = 700;

    const $panels = $('.question-panel');
    const $palette = $('.palette-btn');
    const $status = $('#save-status');
    const $timer = $('#timer');

    let currentIndex = Math.min(Math.max(config.currentQuestionIndex || 0, 0), Math.max($panels.length - 1, 0));
    let remaining = config.remainingSeconds;
    let lastSyncAt = Date.now();
    let pending = loadPending();
    let saving = false;
    let finished = false;
    const timers = {};

    function loadPending() {
        try { return JSON.parse(localStorage.getItem(storageKey)) || {}; } catch (e) { return {}; }
    }

    function persistPending() {
        try {
            if (Object.keys(pending).length === 0) { localStorage.removeItem(storageKey); }
            else { localStorage.setItem(storageKey, JSON.stringify(pending)); }
        } catch (e) { /* storage unavailable */ }
    }

    function setStatus(text, css) {
        $status.attr('class', 'small ' + (css || 'text-muted')).text(text);
    }

    function elapsedSeconds() {
        return Math.max(0, Math.round((Date.now() - lastSyncAt) / 1000));
    }

    function panelAt(index) { return $panels.filter('[data-index="' + index + '"]'); }

    function readAnswer($panel) {
        const type = $panel.data('type');
        const answer = { questionId: Number($panel.data('question-id')), selectedOptionId: null, textAnswer: null, answerJson: null };
        switch (type) {
            case 'MULTIPLE_CHOICE':
            case 'TRUE_FALSE': {
                const v = $panel.find('input[type=radio]:checked').val();
                answer.selectedOptionId = v ? Number(v) : null;
                break;
            }
            case 'MULTIPLE_SELECT': {
                const ids = $panel.find('input[type=checkbox]:checked').map(function () { return Number(this.value); }).get();
                answer.answerJson = ids.length ? JSON.stringify({ selectedOptionIds: ids }) : null;
                break;
            }
            case 'FILL_BLANK':
            case 'SHORT_ANSWER': {
                const t = ($panel.find('.answer-text').val() || '').trim();
                answer.textAnswer = t || null;
                break;
            }
            case 'MATCHING': {
                const matches = {};
                let any = false;
                $panel.find('.match-select').each(function () {
                    if (this.value) { matches[$(this).data('option-id')] = this.value; any = true; }
                });
                answer.answerJson = any ? JSON.stringify({ matches: matches }) : null;
                break;
            }
            case 'ORDERING': {
                const order = $panel.find('.ordering-list > li').map(function () { return Number($(this).data('option-id')); }).get();
                answer.answerJson = JSON.stringify({ orderedOptionIds: order });
                break;
            }
        }
        return answer;
    }

    function isAnswered(answer) {
        return answer.selectedOptionId !== null || !!answer.textAnswer || !!answer.answerJson;
    }

    function refreshProgress() {
        let count = 0;
        $panels.each(function () {
            const answered = $(this).data('answered') === true;
            if (answered) { count++; }
            $palette.filter('[data-goto="' + $(this).data('index') + '"]').toggleClass('answered', answered);
        });
        $('#answered-count').text(count);
        return count;
    }

    function markAnswered($panel) {
        $panel.data('answered', isAnswered(readAnswer($panel)));
    }

    function show(index) {
        if (index < 0 || index >= $panels.length) { return; }
        currentIndex = index;
        $panels.addClass('d-none');
        const $panel = panelAt(index).removeClass('d-none');
        $palette.removeClass('current').filter('[data-goto="' + index + '"]').addClass('current');
        $panel.find('.btn-nav[data-step="-1"]').prop('disabled', index === 0);
        $panel.find('.btn-nav[data-step="1"]').text(index === $panels.length - 1 ? 'Xem lại & nộp bài' : 'Câu sau →');
        $('audio, video').each(function () { if (!$panel.has(this).length) { this.pause(); } });
    }

    function queue(questionId, answer) {
        pending[questionId] = answer;
        persistPending();
        setStatus('Chưa lưu...', 'text-warning');
        clearTimeout(timers[questionId]);
        timers[questionId] = setTimeout(flush, DEBOUNCE_MS);
    }

    function send(payload) {
        const sentAt = Date.now();
        payload.examAttemptId = config.examAttemptId;
        payload.timeSpentSeconds = elapsedSeconds();
        payload.currentQuestionIndex = currentIndex;
        payload.currentQuestionId = Number(panelAt(currentIndex).data('question-id')) || null;
        return CambridgeApp.post(config.saveUrl, payload).then(function (result) {
            if (result.isExpired) { return expire(result.message); }
            if (!result.success) { throw new Error(result.message || 'save failed'); }
            lastSyncAt = sentAt;
            remaining = result.remainingSeconds;
            setStatus('Đã lưu lúc ' + new Date().toLocaleTimeString('vi-VN'), 'text-success');
            return result;
        });
    }

    function flush() {
        if (saving || finished) { return Promise.resolve(); }
        const ids = Object.keys(pending);
        if (ids.length === 0) { return Promise.resolve(); }
        saving = true;
        setStatus('Đang lưu...', 'text-muted');
        const id = ids[0];
        const answer = pending[id];
        return send($.extend({}, answer)).then(function () {
            if (pending[id] === answer) { delete pending[id]; }
            persistPending();
            saving = false;
            return flush();
        }).catch(function () {
            saving = false;
            setStatus(navigator.onLine ? 'Lỗi lưu – sẽ thử lại' : 'Mất kết nối – câu trả lời được giữ trên máy', 'text-danger');
        });
    }

    function heartbeat() {
        if (finished) { return; }
        if (Object.keys(pending).length) { flush(); return; }
        if (saving) { return; }
        saving = true;
        send({ questionId: null }).catch(function () {
            setStatus('Mất kết nối – sẽ thử lại', 'text-danger');
        }).finally(function () { saving = false; });
    }

    function expire(message) {
        finished = true;
        localStorage.removeItem(storageKey);
        alert(message || 'Đã hết thời gian làm bài. Bài thi đã được nộp tự động.');
        window.location.href = config.resultUrl;
    }

    function tick() {
        if (finished) { return; }
        const left = Math.max(0, remaining - elapsedSeconds());
        const h = Math.floor(left / 3600), m = Math.floor((left % 3600) / 60), s = left % 60;
        $timer.text((h ? h + ':' : '') + String(m).padStart(2, '0') + ':' + String(s).padStart(2, '0'));
        $timer.toggleClass('warning', left <= 60);
        if (left <= 0) {
            finished = true;
            setStatus('Hết giờ – đang nộp bài...', 'text-danger');
            const pendingSave = Object.keys(pending).length ? (finished = false, flush().then(function () { finished = true; })) : Promise.resolve();
            pendingSave.finally(function () { $('#submit-form').trigger('submit'); });
        }
    }

    function pauseBeacon() {
        if (finished) { return; }
        const form = new FormData();
        form.append('examAttemptId', config.examAttemptId);
        form.append('currentQuestionIndex', currentIndex);
        form.append('timeSpentSeconds', elapsedSeconds());
        form.append('__RequestVerificationToken', CambridgeApp.token());
        if (navigator.sendBeacon) { navigator.sendBeacon(config.pauseUrl, form); }
    }

    // Restore answers that were not yet synced (e.g. connection lost before leaving).
    Object.keys(pending).forEach(function (id) {
        const a = pending[id];
        const $panel = $panels.filter('[data-question-id="' + id + '"]');
        if (!$panel.length) { delete pending[id]; return; }
        if (a.selectedOptionId !== null) { $panel.find('input[type=radio][value="' + a.selectedOptionId + '"]').prop('checked', true); }
        if (a.textAnswer !== null) { $panel.find('.answer-text').val(a.textAnswer); }
        if (a.answerJson) {
            try {
                const p = JSON.parse(a.answerJson);
                if (p.selectedOptionIds) { $panel.find('input[type=checkbox]').each(function () { this.checked = p.selectedOptionIds.indexOf(Number(this.value)) >= 0; }); }
                if (p.matches) { $panel.find('.match-select').each(function () { $(this).val(p.matches[$(this).data('option-id')] || ''); }); }
                if (p.orderedOptionIds) {
                    const $list = $panel.find('.ordering-list');
                    p.orderedOptionIds.forEach(function (oid) { $list.append($list.children('[data-option-id="' + oid + '"]')); });
                }
            } catch (e) { /* ignore malformed local data */ }
        }
    });

    $panels.each(function () {
        const $panel = $(this);
        const saved = config.savedAnswers.find(function (s) { return s.questionId === Number($panel.data('question-id')); });
        $panel.data('answered', !!(saved && saved.isAnswered) || !!pending[$panel.data('question-id')]);
    });
    persistPending();

    $panels.on('change input', 'input, textarea, select', function () {
        const $panel = $(this).closest('.question-panel');
        markAnswered($panel);
        refreshProgress();
        queue($panel.data('question-id'), readAnswer($panel));
    });

    $panels.on('click', '.btn-move', function () {
        const $item = $(this).closest('li');
        if (Number($(this).data('dir')) < 0) { $item.prev().before($item); } else { $item.next().after($item); }
        const $panel = $item.closest('.question-panel');
        $panel.data('answered', true);
        refreshProgress();
        queue($panel.data('question-id'), readAnswer($panel));
    });

    $panels.on('click', '.btn-nav', function () {
        const next = currentIndex + Number($(this).data('step'));
        if (next >= $panels.length) { $('#btn-submit').trigger('click'); return; }
        show(next);
    });

    $palette.on('click', function () { show(Number($(this).data('goto'))); });

    $('#submitModal').on('show.bs.modal', function () {
        const unanswered = $panels.length - refreshProgress();
        $('#submit-summary').text(unanswered > 0
            ? 'Bạn còn ' + unanswered + ' câu chưa trả lời. Bạn có chắc muốn nộp bài?'
            : 'Bạn đã trả lời tất cả câu hỏi. Nộp bài ngay?');
    });

    $('#submit-form').on('submit', function (e) {
        if ($(this).data('ready')) { return; }
        e.preventDefault();
        const form = this;
        $(form).find('button').prop('disabled', true).text('Đang nộp...');
        const wasFinished = finished;
        finished = false;
        flush().finally(function () {
            finished = true;
            localStorage.removeItem(storageKey);
            $(form).data('ready', true);
            form.submit();
        });
        finished = wasFinished && finished;
    });

    $('#btn-pause').on('click', function () {
        const $btn = $(this).prop('disabled', true).text('Đang lưu...');
        flush().finally(function () {
            const body = new URLSearchParams({
                examAttemptId: config.examAttemptId,
                currentQuestionIndex: currentIndex,
                timeSpentSeconds: elapsedSeconds(),
                __RequestVerificationToken: CambridgeApp.token()
            });
            fetch(config.pauseUrl, { method: 'POST', credentials: 'same-origin', body: body })
                .then(function () { finished = true; window.location.href = config.exitUrl; })
                .catch(function () { $btn.prop('disabled', false).text('Tạm dừng & thoát'); setStatus('Không thể tạm dừng – kiểm tra kết nối', 'text-danger'); });
        });
    });

    window.addEventListener('online', function () { setStatus('Đã kết nối lại – đang đồng bộ...', 'text-muted'); flush(); });
    window.addEventListener('offline', function () { setStatus('Mất kết nối – câu trả lời được giữ trên máy', 'text-danger'); });
    window.addEventListener('pagehide', pauseBeacon);

    show(currentIndex);
    refreshProgress();
    tick();
    setInterval(tick, 1000);
    setInterval(heartbeat, HEARTBEAT_MS);
    flush();
})(jQuery);
