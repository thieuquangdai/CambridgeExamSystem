(function ($) {
    'use strict';

    function token() {
        return $('#antiforgery-form input[name="__RequestVerificationToken"]').val() || '';
    }

    function post(url, data) {
        return fetch(url, {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token() },
            body: data === undefined ? null : JSON.stringify(data)
        }).then(function (response) {
            return response.json().catch(function () { return {}; }).then(function (body) {
                if (!response.ok && body && typeof body === 'object') {
                    body.success = false;
                }
                return body;
            });
        });
    }

    function translate(text) {
        return post('/Translation/Translate', { text: text, sourceLanguage: 'en', targetLanguage: 'vi' })
            .catch(function () { return { success: false, errorMessage: 'Không thể kết nối dịch vụ dịch.' }; });
    }

    function renderTranslation(result, onSaved) {
        const $box = $('<div>');
        if (!result || !result.success) {
            return $box.addClass('text-danger').text((result && result.errorMessage) || 'Không dịch được.');
        }
        $box.append($('<div class="fw-semibold">').text(result.originalText));
        if (result.phonetic) { $box.append($('<div class="text-muted small">').text(result.phonetic)); }
        $box.append($('<div class="text-cambridge fs-6">').text(result.translatedText || ''));
        if (result.definitionText) { $box.append($('<div class="small mt-1">').text(result.definitionText)); }
        if (result.exampleText) { $box.append($('<div class="small fst-italic text-muted">').text('“' + result.exampleText + '”')); }
        if (result.audioUrl) { $box.append($('<audio controls preload="none" class="w-100 mt-1">').attr('src', result.audioUrl)); }
        const $save = $('<button type="button" class="btn btn-sm btn-outline-success mt-2">Lưu từ vựng</button>');
        $save.on('click', function () {
            $save.prop('disabled', true).text('Đang lưu...');
            post('/Translation/Save', {
                originalText: result.originalText,
                translatedText: result.translatedText,
                definitionText: result.definitionText,
                exampleText: result.exampleText,
                phonetic: result.phonetic,
                audioUrl: result.audioUrl
            }).then(function (r) {
                $save.text(r.success ? 'Đã lưu ✓' : (r.message || 'Lỗi khi lưu'));
                if (r.success && onSaved) { onSaved(r); }
            }).catch(function () { $save.prop('disabled', false).text('Thử lại'); });
        });
        return $box.append($save);
    }

    function formatLocalTimes(root) {
        $(root || document).find('time[data-utc]').each(function () {
            const value = $(this).attr('data-utc');
            const date = new Date(value);
            $(this).text(value && !isNaN(date) ? date.toLocaleString('vi-VN') : '—');
        });
    }

    function initSelectionTranslate() {
        if ($('body').data('authenticated') !== true) { return; }
        const $pop = $('#translate-popover');
        const $result = $pop.find('.translate-result');
        const $btn = $pop.find('[data-action="translate"]');
        let selected = '';

        $(document).on('mouseup touchend', '.translatable', function (e) {
            setTimeout(function () {
                const text = (window.getSelection() || '').toString().trim();
                if (!text || text.length > 200 || !/[A-Za-z]/.test(text)) { return; }
                selected = text;
                $result.addClass('d-none').empty();
                $btn.removeClass('d-none');
                const x = (e.pageX || (e.originalEvent.changedTouches && e.originalEvent.changedTouches[0].pageX) || 0);
                const y = (e.pageY || (e.originalEvent.changedTouches && e.originalEvent.changedTouches[0].pageY) || 0);
                $pop.css({ left: Math.max(8, Math.min(x, $(window).width() - 300)), top: y + 12 }).removeClass('d-none');
            }, 10);
        });

        $(document).on('mousedown', function (e) {
            if (!$(e.target).closest('#translate-popover').length) { $pop.addClass('d-none'); }
        });

        $btn.on('click', function () {
            $btn.addClass('d-none');
            $result.removeClass('d-none').text('Đang dịch...');
            translate(selected).then(function (r) { $result.empty().append(renderTranslation(r)); });
        });
    }

    window.CambridgeApp = { token: token, post: post, translate: translate, renderTranslation: renderTranslation, formatLocalTimes: formatLocalTimes };

    $(function () {
        formatLocalTimes();
        initSelectionTranslate();
    });
})(jQuery);
