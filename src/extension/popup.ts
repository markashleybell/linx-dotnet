import { hideStatus, Setting, settings, showInfoStatus, showErrorStatus, showSuccessStatus } from './common';

interface PageDetails {
    title: string;
    url: string;
    abstract: string;
}

const form = document.getElementById("bookmark-details") as HTMLFormElement;

const status = document.getElementById("status") as HTMLSpanElement;

const titleInput = document.getElementById("title") as HTMLInputElement;
const urlInput = document.getElementById("url") as HTMLInputElement;
const abstractInput = document.getElementById("abstract") as HTMLInputElement;
const tagsInput = document.getElementById("tags") as HTMLInputElement;

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

window.addEventListener("load", () => {
    chrome.storage.sync.get([...settings.keys()], stored => {
        form.addEventListener("submit", async (event: Event) => {
            event.preventDefault();
        
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
                // alert(JSON.stringify(e));
                showErrorStatus(status, "Error");
            }
        });
    });

    // // Get the event page
    // chrome.runtime.getBackgroundPage((eventPage) => {
    //     // Call the getPageInfo function in the event page, passing in our onPageDetailsReceived
    //     // function as the callback. This injects content.js into the current tab's HTML
    //     // eventPage.LinxChromeExtension.getPageDetails(onPageDetailsReceived);
    // });
});
