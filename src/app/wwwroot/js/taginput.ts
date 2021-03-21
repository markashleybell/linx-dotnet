import { tagItemTemplate } from './common';
import { TagInput } from 'mab-bootstrap-taginput';

declare const _ALL_TAGS: string[];

const tagInputElement = document.getElementsByClassName('tag-input')[0];

new TagInput<string>({
    input: (tagInputElement as HTMLElement),
    data: _ALL_TAGS || [],
    maxNumberOfSuggestions: 5,
    getId: item => item,
    getLabel: item => item,
    newItemFactory: label => Promise.resolve(label),
    itemTemplate: tagItemTemplate
});
