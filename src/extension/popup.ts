import { hideStatus, Setting, settings, showInfoStatus, showErrorStatus, showSuccessStatus } from './common';

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

const form = document.getElementById("bookmark-details") as HTMLFormElement;

const status = document.getElementById("status") as HTMLSpanElement;

const titleInput = document.getElementById("title") as HTMLInputElement;
const urlInput = document.getElementById("url") as HTMLInputElement;
const tagsInput = document.getElementById("tags") as HTMLInputElement;
const abstractInput = document.getElementById("abstract") as HTMLInputElement;

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

async function post(url: string, options: RequestInit) {
    const response = await fetch(url, options);
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

chrome.runtime.onMessage.addListener(onPageDetailsReceived);

window.addEventListener("load", () => {
    chrome.storage.sync.get([...settings.keys()], stored => {
        chrome.tabs.executeScript({ file: "content.js" });

        form.addEventListener("submit", async (event: Event) => {
            event.preventDefault();
        
            for (const k in inputs) {
                inputs[k].classList.remove('is-invalid');
            }

            showInfoStatus(status, "Saving...");
        
            try {
                const response = await post(stored[Setting.ApiUrl], {
                    method: "POST",
                    body: new FormData(form),
                    headers: {
                        ApiKey: stored[Setting.ApiKey],
                    },
                });
        
                showSuccessStatus(status, "Saved", 1000);
                
                // window.setTimeout(window.close, 1000);
            } catch (e) {
                const errors = (e as ApiErrorResponse).errors;

                for (const error of errors) {
                    const input = inputs[error.key];
                    (input.nextElementSibling as HTMLDivElement).innerText = error.value.join('');
                    input.classList.add('is-invalid');
                }

                showErrorStatus(status, "Error");
            }
        });
    });
});
