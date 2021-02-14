export interface LinxChromeExtension {
    getPageDetails(callback: (message: any) => void): void;
}

export interface Window {
    LinxChromeExtension: LinxChromeExtension;
}

export enum Setting {
    ApiUrl = 'api-url',
    ApiKey = 'api-key'
}

export interface SettingMetadata {
    name: string;
}

export const settings: Map<Setting, SettingMetadata> = new Map();

settings.set(Setting.ApiUrl, { name: 'API Endpoint URL' });
settings.set(Setting.ApiKey, { name: 'API Key' });

export function hideStatus(element: HTMLElement) {
    element.classList.add("status-hidden");
}

function showStatus(element: HTMLElement, message: string, statusClass: string, autoHideAfterMs?: number, onAfterHide?: () => void) {
    element.classList.remove(
        "status-hidden",
        "bg-secondary",
        "bg-success",
        "bg-danger"
    );

    element.classList.add(statusClass);
    element.innerText = message;

    if (autoHideAfterMs) {
        window.setTimeout(() => {
            hideStatus(element);
            if (onAfterHide) {
                onAfterHide();
            }
        }, autoHideAfterMs);
    }
}

export function showInfoStatus(element: HTMLElement, message: string, autoHideAfterMs?: number, onAfterHide?: () => void) {
    showStatus(element, message, "bg-secondary", autoHideAfterMs, onAfterHide);
}

export function showSuccessStatus(element: HTMLElement, message: string, autoHideAfterMs?: number, onAfterHide?: () => void) {
    showStatus(element, message, "bg-success", autoHideAfterMs, onAfterHide);
}

export function showErrorStatus(element: HTMLElement, message: string, autoHideAfterMs?: number, onAfterHide?: () => void) {
    showStatus(element, message, "bg-danger", autoHideAfterMs, onAfterHide);
}
