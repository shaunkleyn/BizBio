(function($) {
    // ... (showToast function remains the same) ...
    function showToast(message) {
        // Create the toast element
        var $toast = $('<div class="toast-notification"></div>')
            .text(message)
            .appendTo('body');

        // Show and hide with a delay
        setTimeout(function() {
            $toast.addClass('show');
        }, 10); // Short delay to trigger CSS transition

        setTimeout(function() {
            $toast.removeClass('show');
            // Remove from DOM after the hide transition
            $toast.on('transitionend', function() {
                $(this).remove();
            });
        }, 2000); // 2 seconds visible + fade out time
    }

    // The core plugin function
    $.fn.contextPopup = function(options) {
        // Define default options
        var settings = $.extend({
            copyText: 'Copy Text',
            proceedText: 'Proceed to Target',
            toastMessage: 'Copied to clipboard!',
            popupId: 'context-popup-menu'
        }, options);

        // Create or find the context menu popup once
        var $popup = $('#' + settings.popupId);
        if ($popup.length === 0) {
            $popup = $('<div id="' + settings.popupId + '" class="context-popup"></div>');
            $popup.html(
                '<button class="copy-action"></button>' +
                '<button class="proceed-action"></button>'
            );
            $('body').append($popup);
        }

        // --- REVISION 1: Document Click Handler Update ---
        // Handle clicks on the document to hide the menu
        $(document).on('click', function(e) {
            // Check if the click was outside the popup AND outside the anchor that might have just opened it
            if (!$(e.target).closest('#' + settings.popupId).length && !$(e.target).closest(this).hasClass('context-link')) {
                 // Only hide if the popup is currently visible
                if ($popup.is(':visible')) {
                    $popup.hide();
                }
            }
        });
        // ---------------------------------------------------

        // Loop through the selected elements
        return this.each(function() {
            var $anchor = $(this);
            var anchorText = $anchor.text();
            var anchorHref = $anchor.attr('href');

            $anchor.on('click', function(e) {
                e.preventDefault(); // Stop default navigation
                // --- REVISION 2: Stop Propagation ---
                e.stopPropagation(); // Prevent the click from immediately reaching the document handler and hiding the menu
                // ------------------------------------

                // Update button text
                $popup.find('.copy-action').text(settings.copyText);
                $popup.find('.proceed-action').text(settings.proceedText);

                // Position the popup near the anchor
                var offset = $anchor.offset();
                $popup.css({
                    top: offset.top + $anchor.outerHeight() + 5, // Below the link
                    left: offset.left,
                    display: 'block'
                });

                // Clear previous handlers and attach new ones based on the current anchor
                $popup.off('click');

                // Copy Action
                $popup.on('click', '.copy-action', function() {
                    // ... (Copy logic remains the same) ...
                    if (navigator.clipboard && navigator.clipboard.writeText) {
                        navigator.clipboard.writeText(anchorText).then(function() {
                            showToast(settings.toastMessage);
                            $popup.hide();
                        }).catch(function(err) {
                            console.error('Could not copy text: ', err);
                        });
                    } else {
                        alert('Clipboard API not supported in this browser.');
                    }
                });

                // Proceed Action
                $popup.on('click', '.proceed-action', function() {
                    $popup.hide();
                    window.location.href = anchorHref; // Execute the original href
                });
            });
        });
    };
})(jQuery);