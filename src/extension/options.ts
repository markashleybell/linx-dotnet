const postUrlInput = document.getElementById('post_url') as HTMLInputElement;
const tagJsonUrlInput = document.getElementById('tag_json_url') as HTMLInputElement;

// Saves options to localStorage.
function save_options() {
    localStorage['post_url'] = postUrlInput.value;
    localStorage['tag_json_url'] = tagJsonUrlInput.value;
    // Update status to let user know options were saved.
    var status = document.getElementById('status');
    status.innerHTML = 'Options Saved.';
    setTimeout(function() {
        status.innerHTML = '';
    }, 750);
}

// Restores select box state to saved value from localStorage.
function restore_options() {
    var postUrl = localStorage['post_url'];
    if (!postUrl) {
        return;
    }
    postUrlInput.value = postUrl;
    var tagJsonUrl = localStorage['tag_json_url'];
    if (!tagJsonUrl) {
        return;
    }
    tagJsonUrlInput.value = tagJsonUrl;
}

document.addEventListener('DOMContentLoaded', restore_options);
document.querySelector('#save').addEventListener('click', save_options);