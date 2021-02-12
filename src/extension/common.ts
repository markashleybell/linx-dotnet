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

function showStatus(element: HTMLElement, message: string, statusClass: string, autoHideDelay?: number) {
    element.classList.remove(
        "status-hidden",
        "bg-secondary",
        "bg-success",
        "bg-danger"
    );

    element.classList.add(statusClass);
    element.innerText = message;

    if (autoHideDelay) {
        window.setTimeout(() => hideStatus(element), autoHideDelay);
    }
}

export function showInfoStatus(element: HTMLElement, message: string, autoHideDelay?: number) {
    showStatus(element, message, "bg-secondary", autoHideDelay);
}

export function showSuccessStatus(element: HTMLElement, message: string, autoHideDelay?: number) {
    showStatus(element, message, "bg-success", autoHideDelay);
}

export function showErrorStatus(element: HTMLElement, message: string, autoHideDelay?: number) {
    showStatus(element, message, "bg-danger", autoHideDelay);
}
