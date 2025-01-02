﻿using Microsoft.Extensions.Logging;

namespace PhotonPiano.Test.Extensions
{
    public static class DistributedApplicationExtensions
    {
        /// <summary>
        /// Waits for the specified resource to reach the specified state.
        /// </summary>
        public static Task WaitForResource(this DistributedApplication app, string resourceName, string? targetState = null,
            CancellationToken cancellationToken = default)
        {
            targetState ??= KnownResourceStates.Running;
            var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

            return resourceNotificationService.WaitForResourceAsync(resourceName, targetState, cancellationToken);
        }

        /// <summary>
        /// Waits for all resources in the application to reach one of the specified states.
        /// </summary>
        /// <remarks>
        /// If <paramref name="targetStates"/> is null, the default states are <see cref="KnownResourceStates.Running"/> and <see cref="KnownResourceStates.Hidden"/>.
        /// </remarks>
        public static async Task WaitForResourcesAsync(this DistributedApplication app,
            IEnumerable<string>? targetStates = null, CancellationToken cancellationToken = default)
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(WaitForResourcesAsync));

            targetStates ??=
                [KnownResourceStates.Running, KnownResourceStates.Hidden, .. KnownResourceStates.TerminalStates];
            var applicationModel = app.Services.GetRequiredService<DistributedApplicationModel>();
            var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

            var resourceTasks = new Dictionary<string, Task<(string Name, string State)>>();

            foreach (var resource in applicationModel.Resources)
            {
                resourceTasks[resource.Name] = GetResourceWaitTask(resource.Name, targetStates, cancellationToken);
            }

            logger.LogInformation("Waiting for resources [{Resources}] to reach one of target states [{TargetStates}].",
                string.Join(',', resourceTasks.Keys),
                string.Join(',', targetStates));

            while (resourceTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(resourceTasks.Values);
                var (completedResourceName, targetStateReached) = await completedTask;

                if (targetStateReached == KnownResourceStates.FailedToStart)
                    throw new DistributedApplicationException($"Resource '{completedResourceName}' failed to start.");

                resourceTasks.Remove(completedResourceName);

                logger.LogInformation("Wait for resource '{ResourceName}' completed with state '{ResourceState}'",
                    completedResourceName, targetStateReached);

                // Ensure resources being waited on still exist
                var remainingResources = resourceTasks.Keys.ToList();
                for (var i = remainingResources.Count - 1; i > 0; i--)
                {
                    var name = remainingResources[i];
                    if (applicationModel.Resources.All(r => r.Name != name))
                    {
                        logger.LogInformation("Resource '{ResourceName}' was deleted while waiting for it.", name);
                        resourceTasks.Remove(name);
                        remainingResources.RemoveAt(i);
                    }
                }

                if (resourceTasks.Count > 0)
                {
                    logger.LogInformation(
                        "Still waiting for resources [{Resources}] to reach one of target states [{TargetStates}].",
                        string.Join(',', remainingResources),
                        string.Join(',', targetStates));
                }
            }

            logger.LogInformation("Wait for all resources completed successfully!");

            async Task<(string Name, string State)> GetResourceWaitTask(string resourceName,
                IEnumerable<string> targetStates, CancellationToken cancellationToken)
            {
                var state = await resourceNotificationService.WaitForResourceAsync(resourceName, targetStates,
                    cancellationToken);
                return (resourceName, state);
            }
        }
    }
}
