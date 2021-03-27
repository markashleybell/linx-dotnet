import {
    LinkExistsResponse,
    onMessageReceived,
    PageDetailsResponse,
    post,
    settings,
    validateSettings,
} from './common';

const currentUrl = window.location.href;

let linxExistsResponse = new LinkExistsResponse(currentUrl, false);

onMessageReceived('linkexistsrequest', () => {
    chrome.runtime.sendMessage(linxExistsResponse);
});

onMessageReceived('pagedetailsrequest', () => {
    const pageDetailsResponse = new PageDetailsResponse(
        document.title,
        window.location.href,
        window.getSelection().toString()
    );

    chrome.runtime.sendMessage(pageDetailsResponse);
});

window.addEventListener('load', () => {
    chrome.storage.sync.get([...settings.keys()], async (stored) => {
        const [settingsAreValid, apiUrl, apiKey] = validateSettings(stored);

        if (!settingsAreValid) {
            return;
        }

        const checkUrl = apiUrl + '/check';

        var formData = new FormData();

        formData.append('Url', currentUrl);

        const response = await post(checkUrl, {
            method: 'POST',
            body: formData,
            headers: {
                ApiKey: apiKey,
            },
        });

        // Cache the result of the query; if the user switches
        // back to this tab, we just return the cached result
        // rather than doing a new remote call every time
        linxExistsResponse = new LinkExistsResponse(currentUrl, response.exists);

        chrome.runtime.sendMessage(linxExistsResponse);
    });
});
