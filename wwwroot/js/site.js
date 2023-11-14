// Function to copy the unique link to the clipboard
document.getElementById("copyButton").addEventListener("click", function () {
    var uniqueLink = document.getElementById("uniqueLink");
    uniqueLink.select();
    document.execCommand("copy");
    alert("Link copied to clipboard: " + uniqueLink.value);
});

// Function to create and open a shareable link for Facebook
document.getElementById("facebookShare").addEventListener("click", function () {
    var uniqueLink = document.getElementById("uniqueLink").value;
    var facebookShareUrl = "https://www.facebook.com/sharer/sharer.php?u=" + encodeURIComponent(uniqueLink);
    window.open(facebookShareUrl, "_blank");
});

// Function to create and open a shareable link for TikTok
document.getElementById("tiktokShare").addEventListener("click", function () {
    var uniqueLink = document.getElementById("uniqueLink").value;
    var tiktokShareUrl = "https://vm.tiktok.com/ZMeW9B1T4/?u=" + encodeURIComponent(uniqueLink);
    window.open(tiktokShareUrl, "_blank");
});

// Function to create and open a shareable link for WhatsApp
document.getElementById("whatsappShare").addEventListener("click", function () {
    var uniqueLink = document.getElementById("uniqueLink").value;
    var whatsappShareUrl = "https://api.whatsapp.com/send?text=" +
        encodeURIComponent("Send me an anonymous message of about 1000 characters on Private messaging service (PMS) with an option to include an image: " + uniqueLink);
    window.open(whatsappShareUrl, "_blank");
});

function toggleImageSection() {
    var imageSection = document.getElementById("imageSection");
    var addImageButton = document.getElementById("addImageButton");

    if (imageSection.style.display === "none")
    {
        imageSection.style.display = "block";
        addImageButton.style.display = "none"; // Hide the button
    }
    else
    {
        imageSection.style.display = "none";
        addImageButton.style.display = "block"; // Show the button
    }
}
function updateCharacterCount() {
    const messageInput = document.getElementById("messageInput");
    const characterCount = document.getElementById("characterCount");

    const currentLength = messageInput.value.length;
    const maxLength = 1000;
    const charactersLeft = maxLength - currentLength;

    characterCount.textContent = `Characters left: ${charactersLeft}`;
}
