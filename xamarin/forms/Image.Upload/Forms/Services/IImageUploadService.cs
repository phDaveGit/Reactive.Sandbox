﻿using System;

namespace Forms.Services
{
    public interface IImageUploadService
    {
        // When the user clicks the upload
        // When the internet is available
        // When the item is added to the queue
        // When the upload starts
        // When the upload finishes
        // Whether the upload is successful
        // When something is removed from the queue
        // When the user turns the service on
        // When the user turns the service off
        
        /// <summary>
        /// An observable sequence that notifies of changes to the internal queue.
        /// </summary>
        IObservable<UploadEventArgs> Queued { get; }

        /// <summary>
        /// Adds a payload to the queue.
        /// </summary>
        /// <param name="payload"></param>
        void Queue(MyTestPayload payload);

        /// <summary>
        /// Toggles the service on/off.
        /// </summary>
        /// <returns></returns>
        IObservable<bool> ToggleService();
    }
}