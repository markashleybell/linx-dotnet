import { TagInput } from 'mab-bootstrap-taginput';

declare const _ALL_TAGS: string[];

const tagInputElement = document.getElementsByClassName('tag-input')[0];

new TagInput<string>({
    input: (tagInputElement as HTMLElement),
    data: _ALL_TAGS || [],
    getId: item => item,
    getLabel: item => item,
    newItemFactory: label => Promise.resolve(label),
    itemTemplate: '<div class="{{globalCssClassPrefix}}-tag" data-id="{{id}}" data-label="{{label}}">{{label}} <i class="{{globalCssClassPrefix}}-removetag bi bi-x"></i></div>'
});
