import { hideStatus, Setting, settings, showInfoStatus, showErrorStatus, showSuccessStatus } from './common';
import { TagInput } from 'mab-bootstrap-taginput';

interface PageDetails {
    title: string;
    url: string;
    abstract: string;
}

interface ApiValidationError {
    key: string;
    value: string[];
}

interface ApiErrorResponse {
    errors: ApiValidationError[];
}

const form = document.getElementById('bookmark-details') as HTMLFormElement;
const status = document.getElementById('status') as HTMLSpanElement;
const titleInput = document.getElementById('title') as HTMLInputElement;
const urlInput = document.getElementById('url') as HTMLInputElement;
const tagsInput = document.getElementById('tags') as HTMLInputElement;
const abstractInput = document.getElementById('abstract') as HTMLInputElement;

const inputs: Record<string, HTMLInputElement> = {
    'Title': titleInput,
    'Url': urlInput,
    'Tags': tagsInput,
    'Abstract': abstractInput
}

// This callback function is called when the content script
// has been injected and returned its results
function onPageDetailsReceived(pageDetails: PageDetails) {
    titleInput.value = pageDetails.title;
    urlInput.value = pageDetails.url;
    abstractInput.value = pageDetails.abstract;
}

function validateSettings(settings: { [key: string]: any }): [boolean, string, string] {
    const apiUrl = settings[Setting.ApiUrl]?.trim();
    const apiKey = settings[Setting.ApiKey]?.trim();

    return apiUrl && apiKey 
        ? [true, apiUrl, apiKey]
        : [false, null, null];
}

async function post(url: string, options: RequestInit) {
    const response = await fetch(url, options);
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

chrome.runtime.onMessage.addListener(onPageDetailsReceived);

window.addEventListener('load', () => {
    chrome.storage.sync.get([...settings.keys()], async (stored) => {
        const [settingsAreValid, apiUrl, apiKey] = validateSettings(stored);

        if (!settingsAreValid) {
            chrome.runtime.openOptionsPage();
            return;
        }

        chrome.tabs.executeScript({ file: 'content.js' });

        try {
            const tagsUrl = apiUrl + '/tags';

            const response = await post(tagsUrl, {
                method: 'POST',
                headers: {
                    ApiKey: apiKey,
                }
            });
    
            new TagInput<string>({
                input: tagsInput,
                data: response || [],
                getId: item => item,
                getLabel: item => item,
                newItemFactory: label => Promise.resolve(label),
                itemTemplate: '<div class="{{globalCssClassPrefix}}-tag" data-id="{{id}}" data-label="{{label}}">{{label}} <i class="{{globalCssClassPrefix}}-removetag bi bi-x"></i></div>'
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
                    }
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
