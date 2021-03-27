import {
    ApiErrorResponse,
    onMessageReceived,
    PageDetails,
    PageDetailsRequest,
    post,
    settings,
    showInfoStatus,
    showErrorStatus,
    showSuccessStatus,
    validateSettings,
} from './common';

import { TagInput } from 'mab-bootstrap-taginput';

const form = document.getElementById('bookmark-details') as HTMLFormElement;
const status = document.getElementById('status') as HTMLSpanElement;
const titleInput = document.getElementById('title') as HTMLInputElement;
const urlInput = document.getElementById('url') as HTMLInputElement;
const tagsInput = document.getElementById('tags') as HTMLInputElement;
const abstractInput = document.getElementById('abstract') as HTMLInputElement;

const inputs: Record<string, HTMLInputElement> = {
    Title: titleInput,
    Url: urlInput,
    Tags: tagsInput,
    Abstract: abstractInput,
};

onMessageReceived('pagedetails', (msg: PageDetails) => {
    titleInput.value = msg.title;
    urlInput.value = msg.url;
    abstractInput.value = msg.abstract;
});

window.addEventListener('load', () => {
    chrome.storage.sync.get([...settings.keys()], async (stored) => {
        const [settingsAreValid, apiUrl, apiKey] = validateSettings(stored);

        if (!settingsAreValid) {
            chrome.runtime.openOptionsPage();
            return;
        }

        const tabs = await chrome.tabs.query({
            currentWindow: true,
            active: true,
        });

        const currentTab = tabs[0];

        chrome.tabs.sendMessage(currentTab.id, new PageDetailsRequest());

        try {
            const tagsUrl = apiUrl + '/tags';

            const response = await post(tagsUrl, {
                method: 'POST',
                headers: {
                    ApiKey: apiKey,
                },
            });

            new TagInput<string>({
                input: tagsInput,
                data: response || [],
                maxNumberOfSuggestions: 5,
                getId: (item) => item,
                getLabel: (item) => item,
                newItemFactory: (label) => Promise.resolve(label),
                itemTemplate:
                    '<div class="{{globalCssClassPrefix}}-tag" data-id="{{id}}" data-label="{{label}}">{{label}} <i class="{{globalCssClassPrefix}}-removetag bi bi-x"></i></div>',
            });
        } catch (e) {
            const errors = (e as ApiErrorResponse).errors;

            for (const error of errors) {
                const input = inputs[error.key];
                (input.nextElementSibling as HTMLDivElement).innerText = error.value.join('');
                input.classList.add('is-invalid');
            }

            showErrorStatus(status, 'Error');
        }

        form.addEventListener('submit', async (event: Event) => {
            event.preventDefault();

            for (const k in inputs) {
                inputs[k].classList.remove('is-invalid');
            }

            showInfoStatus(status, 'Saving...');

            try {
                const createUrl = apiUrl + '/create';

                const response = await post(createUrl, {
                    method: 'POST',
                    body: new FormData(form),
                    headers: {
                        ApiKey: apiKey,
                    },
                });

                showSuccessStatus(status, 'Saved', 1000, window.close);
            } catch (e) {
                const errors = (e as ApiErrorResponse).errors;

                for (const error of errors) {
                    const input = inputs[error.key];
                    (input.nextElementSibling as HTMLDivElement).innerText = error.value.join('');
                    input.classList.add('is-invalid');
                }

                showErrorStatus(status, 'Error');
            }
        });
    });
});
