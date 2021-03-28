import {
    onMessageReceived,
    icons,
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

chrome.tabs.onActivated.addListener((tab) => {
    chrome.action.disable(tab.tabId);
    chrome.tabs.sendMessage(tab.tabId, new PageStateRequest());
});

chrome.tabs.onUpdated.addListener((tabId, _, tab) => {
    if (tab.url.startsWith('chrome://')) {
        chrome.action.disable(tabId);
    }
});
