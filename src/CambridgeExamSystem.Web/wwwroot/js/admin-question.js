(function ($) {
    'use strict';

    const $type = $('#question-type');
    const $list = $('#options-list');

    function apply() {
        const $selected = $type.find(':selected');
        const code = $selected.data('code');
        const needsOptions = $selected.data('options') === true;
        $('#options-block').toggleClass('d-none', !needsOptions);
        $('#text-answer-block').toggleClass('d-none', needsOptions);
        $list.find('.matching-col').toggleClass('d-none', code !== 'MATCHING');
        $list.find('.correct-col').toggleClass('d-none', code === 'MATCHING' || code === 'ORDERING');
        const hints = {
            MULTIPLE_CHOICE: '(chọn đúng 1 đáp án)',
            TRUE_FALSE: '(2 lựa chọn True/False, chọn 1 đáp án đúng)',
            MULTIPLE_SELECT: '(chọn một hoặc nhiều đáp án đúng)',
            MATCHING: '(mỗi vế trái nhập khoá nối tương ứng)',
            ORDERING: '(nhập theo đúng thứ tự – hệ thống sẽ xáo trộn khi hiển thị)'
        };
        $('#options-hint').text(hints[code] || '');
    }

    $('#add-option').on('click', function () {
        const i = $list.children('.option-row').length;
        const $row = $list.children('.option-row').first().clone();
        $row.find('input').each(function () {
            this.name = this.name.replace(/Options\[\d+\]/, 'Options[' + i + ']');
            if (this.id) { this.id = 'opt-correct-' + i; }
            if (this.type === 'checkbox') { this.checked = false; } else { this.value = ''; }
        });
        $row.find('label').attr('for', 'opt-correct-' + i);
        $row.find('input[type=text], input:not([type])').first().attr('placeholder', 'Lựa chọn ' + (i + 1));
        $list.append($row);
        apply();
    });

    $type.on('change', apply);
    apply();
})(jQuery);
