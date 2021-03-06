// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Imdb.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ngsa.App.Controllers;
using Ngsa.App.Model;
using Ngsa.Middleware;

namespace Ngsa.App
{
    public partial class CosmosHealthCheck : IHealthCheck
    {
        private const int MaxResponseTime = 200;
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Build the response
        /// </summary>
        /// <param name="uri">string</param>
        /// <param name="targetDurationMs">double (ms)</param>
        /// <param name="ex">Exception (default = null)</param>
        /// <param name="data">Dictionary(string, object)</param>
        /// <param name="testName">Test Name</param>
        /// <returns>HealthzCheck</returns>
        private HealthzCheck BuildHealthzCheck(string uri, double targetDurationMs, Exception ex = null, Dictionary<string, object> data = null, string testName = null)
        {
            stopwatch.Stop();

            // create the result
            HealthzCheck result = new HealthzCheck
            {
                Endpoint = uri,
                Status = HealthStatus.Healthy,
                Duration = stopwatch.Elapsed,
                TargetDuration = new System.TimeSpan(0, 0, 0, 0, (int)targetDurationMs),
                ComponentId = testName,
                ComponentType = "datastore",
            };

            // check duration
            if (result.Duration.TotalMilliseconds > targetDurationMs)
            {
                result.Status = HealthStatus.Degraded;
                result.Message = HealthzCheck.TimeoutMessage;
            }

            // add the exception
            if (ex != null)
            {
                result.Status = HealthStatus.Unhealthy;
                result.Message = ex.Message;
            }

            // add the results to the dictionary
            if (data != null && !string.IsNullOrEmpty(testName))
            {
                data.Add(testName + ":responseTime", result);
            }

            return result;
        }

        /// <summary>
        /// Get Genres Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> GetGenresAsync(Dictionary<string, object> data = null)
        {
            const string name = "getGenres";
            const string path = "/api/genres";

            stopwatch.Restart();

            try
            {
                _ = await DataService.Read<List<string>>(path, string.Empty).ConfigureAwait(false);

                return BuildHealthzCheck(path, MaxResponseTime, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }

        /// <summary>
        /// Get Movie by Id Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> GetMovieByIdAsync(string movieId, Dictionary<string, object> data = null)
        {
            const string name = "getMovieById";
            string path = "/api/movies/" + movieId;

            stopwatch.Restart();

            try
            {
                _ = await DataService.Read<Movie>(path, string.Empty).ConfigureAwait(false);

                return BuildHealthzCheck(path, MaxResponseTime / 2, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime / 2, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }

        /// <summary>
        /// Search Movies Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> SearchMoviesAsync(string query, Dictionary<string, object> data = null)
        {
            const string name = "searchMovies";

            MovieQueryParameters movieQuery = new MovieQueryParameters { Q = query };

            string path = "/api/movies?q=" + movieQuery.Q;

            stopwatch.Restart();

            try
            {
                _ = await DataService.Read<List<Movie>>(path, string.Empty).ConfigureAwait(false);

                return BuildHealthzCheck(path, MaxResponseTime, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }

        /// <summary>
        /// Get Actor By Id Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> GetActorByIdAsync(string actorId, Dictionary<string, object> data = null)
        {
            const string name = "getActorById";
            string path = "/api/actors/" + actorId;

            stopwatch.Restart();

            try
            {
                _ = await DataService.Read<Actor>(path, string.Empty).ConfigureAwait(false);

                return BuildHealthzCheck(path, MaxResponseTime / 2, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime / 2, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }

        /// <summary>
        /// Search Actors Healthcheck
        /// </summary>
        /// <returns>HealthzCheck</returns>
        private async Task<HealthzCheck> SearchActorsAsync(string query, Dictionary<string, object> data = null)
        {
            const string name = "searchActors";

            ActorQueryParameters actorQuery = new ActorQueryParameters { Q = query };

            string path = "/api/actors?q=" + actorQuery.Q;

            stopwatch.Restart();

            try
            {
                _ = await DataService.Read<List<Actor>>(path, string.Empty).ConfigureAwait(false);

                return BuildHealthzCheck(path, MaxResponseTime, null, data, name);
            }
            catch (Exception ex)
            {
                BuildHealthzCheck(path, MaxResponseTime, ex, data, name);

                // throw the exception so that HealthCheck logs
                throw;
            }
        }
    }
}
