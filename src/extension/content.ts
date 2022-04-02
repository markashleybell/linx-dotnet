import {
    handleRuntimeError,
    onMessageReceived,
    PageDetailsResponse,
    PageStateResponse,
    post,
    settings,
    validateSettings,
} from './common';

const currentUrl = window.location.href;

let state = new PageStateResponse(currentUrl, false, true);

onMessageReceived('linksavedrequest', () => {
    state = new PageStateResponse(currentUrl, true, false);

    chrome.runtime.sendMessage(state, handleRuntimeError('content: linksavedrequest'));
});

onMessageReceived('pagestaterequest', () => {
    chrome.runtime.sendMessage(state, handleRuntimeError('content: pagestaterequest'));
});

onMessageReceived('pagedetailsrequest', () => {
    const pageDetailsResponse = new PageDetailsResponse(
        document.title,
        window.location.href,
        window.getSelection().toString()
    );

    chrome.runtime.sendMessage(pageDetailsResponse, handleRuntimeError('content: pagedetailsrequest'));
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
        state = new PageStateResponse(currentUrl, response.exists, false);

        chrome.runtime.sendMessage(state, handleRuntimeError('content: load'));
    });
});
