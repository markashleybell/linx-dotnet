import {
    handleRuntimeError,
    onMessageReceived,
    icons,
    isNotBookmarkable,
    PageStateResponse,
    PageStateRequest,
} from './common';

onMessageReceived('pagestateresponse', (msg: PageStateResponse, sender) => {
    if (msg.disabled) {
        chrome.action.disable(sender.tab.id);
        chrome.action.setIcon({ path: icons.disabled, tabId: sender.tab.id });
    } else if (msg.linkExists) {
        chrome.action.disable(sender.tab.id);
        chrome.action.setIcon({ path: icons.green, tabId: sender.tab.id });
    } else {
        chrome.action.enable(sender.tab.id);
        chrome.action.setIcon({ path: icons.enabled, tabId: sender.tab.id });
    }
});

chrome.tabs.onActivated.addListener((activeInfo) => {
    chrome.action.disable(activeInfo.tabId);
    chrome.tabs.sendMessage(activeInfo.tabId, new PageStateRequest(), handleRuntimeError('background: activated'));
});

chrome.tabs.onUpdated.addListener((tabId, _, tab) => {
    if (isNotBookmarkable(tab.url)) {
        chrome.action.disable(tabId);
    }
});
