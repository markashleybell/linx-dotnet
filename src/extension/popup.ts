import { settings } from './common';

interface PageDetails {
    title: string;
    url: string;
    abstract: string;
}

// Get the API URLs from localstorage
const postUrl = localStorage["post_url"];
const tagJsonUrl = localStorage["tag_json_url"];
const apiKey = "FAKEAPIKEY";

const form = document.getElementById("bookmark-details") as HTMLFormElement;

const statusIndicator = document.getElementById("result") as HTMLSpanElement;

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

function hideStatus() {
    statusIndicator.style.display = "none";
}

function showStatus(message: string, statusClass: string) {
    statusIndicator.classList.remove(
        "label-default",
        "label-success",
        "label-danger"
    );
    statusIndicator.classList.add(statusClass);
    statusIndicator.innerText = message;
    statusIndicator.style.display = "inline";
}

function showInfoStatus(message: string) {
    showStatus(message, "label-default");
}

function showSuccessStatus(message: string) {
    showStatus(message, "label-success");
}

function showErrorStatus(message: string) {
    showStatus(message, "label-danger");
}

async function post(url: string, options: RequestInit) {
    const response = await fetch(url, options);
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

form.addEventListener("submit", async (event: Event) => {
    event.preventDefault();

    showInfoStatus("Saving...");

    try {
        const response = await post(postUrl, {
            method: "POST",
            body: new FormData(form),
            headers: {
                ApiKey: apiKey,
            },
        });

        showSuccessStatus("Saved");
        // window.setTimeout(window.close, 1000);
    } catch (e) {
        alert(JSON.stringify(e));
        hideStatus();
    }
});

// When the popup HTML has loaded
window.addEventListener("load", (_) => {
    // Get the event page
    chrome.runtime.getBackgroundPage((eventPage) => {
        // Call the getPageInfo function in the event page, passing in our onPageDetailsReceived
        // function as the callback. This injects content.js into the current tab's HTML
        // eventPage.LinxChromeExtension.getPageDetails(onPageDetailsReceived);
    });

    hideStatus();

    if (!postUrl || !tagJsonUrl) {
        alert("POST Url and Tag JSON Url are not set");
        return;
    }
});
