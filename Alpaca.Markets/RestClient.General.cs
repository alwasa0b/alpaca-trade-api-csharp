﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Alpaca.Markets
{
    public sealed partial class RestClient
    {
        /// <summary>
        /// Gets account information from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only account information.</returns>
        public Task<IAccount> GetAccountAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "account",
            };

            return getSingleObjectAsync<IAccount, JsonAccount>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets account configuration settings from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Mutable version of account configuration object.</returns>
        public Task<IAccountConfiguration> GetAccountConfigurationAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "account/configurations",
            };

            return getSingleObjectAsync<IAccountConfiguration, JsonAccountConfiguration>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Updates account configuration settings using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="accountConfiguration">New account configuration object for updating.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Mutable version of updated account configuration object.</returns>
        public async Task<IAccountConfiguration> PatchAccountConfigurationAsync(
            IAccountConfiguration accountConfiguration,
            CancellationToken cancellationToken = default)
        {
            await _alpacaRestApiThrottler.WaitToProceed(cancellationToken).ConfigureAwait(false);

            using var request = new HttpRequestMessage(_httpMethodPatch,
                new Uri("account/configurations", UriKind.RelativeOrAbsolute));

            var serializer = new JsonSerializer();
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, accountConfiguration);
                request.Content = new StringContent(stringWriter.ToString());
            }

            using var response = await _alpacaHttpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return await deserializeAsync<IAccountConfiguration, JsonAccountConfiguration>(response)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets list of account activities from Alpaca REST API endpoint by specific activity.
        /// </summary>
        /// <param name="activityType">The activity type you want to view entries for.</param>
        /// <param name="date">The date for which you want to see activities.</param>
        /// <param name="until">The response will contain only activities submitted before this date. (Cannot be used with date.)</param>
        /// <param name="after">The response will contain only activities submitted after this date. (Cannot be used with date.)</param>
        /// <param name="direction">The response will be sorted by time in this direction. (Default behavior is descending.)</param>
        /// <param name="pageSize">The maximum number of entries to return in the response.</param>
        /// <param name="pageToken">The ID of the end of your current page of results.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of asset information objects.</returns>
        public Task<IReadOnlyList<IAccountActivity>> ListAccountActivitiesAsync(
            AccountActivityType activityType,
            DateTime? date = null,
            DateTime? until = null,
            DateTime? after = null,
            SortDirection? direction = null,
            Int64? pageSize = null,
            String? pageToken = null,
            CancellationToken cancellationToken = default)
        {
            if (date.HasValue && (until.HasValue || after.HasValue))
            {
                throw new ArgumentException("You unable to specify 'date' and 'until'/'after' arguments in same call.");
            }

            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + $"account/activities/{activityType.ToEnumString()}",
                Query = new QueryBuilder()
                    .AddParameter("date", date, "yyyy-MM-dd")
                    .AddParameter("until", until, "O")
                    .AddParameter("after", after, "O")
                    .AddParameter("direction", direction)
                    .AddParameter("pageSize", pageSize)
                    .AddParameter("pageToken", pageToken)
            };

            return getObjectsListAsync<IAccountActivity, JsonAccountActivity>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets list of account activities from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="activityTypes">The list of activity types you want to view entries for.</param>
        /// <param name="date">The date for which you want to see activities.</param>
        /// <param name="until">The response will contain only activities submitted before this date. (Cannot be used with date.)</param>
        /// <param name="after">The response will contain only activities submitted after this date. (Cannot be used with date.)</param>
        /// <param name="direction">The response will be sorted by time in this direction. (Default behavior is descending.)</param>
        /// <param name="pageSize">The maximum number of entries to return in the response.</param>
        /// <param name="pageToken">The ID of the end of your current page of results.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of asset information objects.</returns>
        public Task<IReadOnlyList<IAccountActivity>> ListAccountActivitiesAsync(
            IEnumerable<AccountActivityType>? activityTypes = null,
            DateTime? date = null,
            DateTime? until = null,
            DateTime? after = null,
            SortDirection? direction = null,
            Int64? pageSize = null,
            String? pageToken = null,
            CancellationToken cancellationToken = default)
        {
            if (date.HasValue && (until.HasValue || after.HasValue))
            {
                throw new ArgumentException("You unable to specify 'date' and 'until'/'after' arguments in same call.");
            }

            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "account/activities",
                Query = new QueryBuilder()
                    .AddParameter("activity_types", activityTypes)
                    .AddParameter("date", date, "yyyy-MM-dd")
                    .AddParameter("until", until, "O")
                    .AddParameter("after", after, "O")
                    .AddParameter("direction", direction)
                    .AddParameter("pageSize", pageSize)
                    .AddParameter("pageToken", pageToken)
            };

            return getObjectsListAsync<IAccountActivity, JsonAccountActivity>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets list of available assets from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="assetStatus">Asset status for filtering.</param>
        /// <param name="assetClass">Asset class for filtering.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of asset information objects.</returns>
        public Task<IReadOnlyList<IAsset>> ListAssetsAsync(
            AssetStatus? assetStatus = null,
            AssetClass? assetClass = null,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "assets",
                Query = new QueryBuilder()
                    .AddParameter("status", assetStatus)
                    .AddParameter("asset_class", assetClass)
            };

            return getObjectsListAsync<IAsset, JsonAsset>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Get single asset information by asset name from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="symbol">Asset name for searching.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only asset information.</returns>
        public Task<IAsset> GetAssetAsync(
            String symbol,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + $"assets/{symbol}",
            };

            return getSingleObjectAsync<IAsset, JsonAsset>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets list of available orders from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="orderStatusFilter">Order status for filtering.</param>
        /// <param name="orderListSorting">The chronological order of response based on the submission time.</param>
        /// <param name="untilDateTimeExclusive">Returns only orders until specified timestamp (exclusive).</param>
        /// <param name="afterDateTimeExclusive">Returns only orders after specified timestamp (exclusive).</param>
        /// <param name="limitOrderNumber">Maximal number of orders in response.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of order information objects.</returns>
        public Task<IReadOnlyList<IOrder>> ListOrdersAsync(
            OrderStatusFilter? orderStatusFilter = null,
            SortDirection? orderListSorting = null,
            DateTime? untilDateTimeExclusive = null,
            DateTime? afterDateTimeExclusive = null,
            Int64? limitOrderNumber = null,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "orders",
                Query = new QueryBuilder()
                    .AddParameter("status", orderStatusFilter)
                    .AddParameter("direction", orderListSorting)
                    .AddParameter("until", untilDateTimeExclusive, "O")
                    .AddParameter("after", afterDateTimeExclusive, "O")
                    .AddParameter("limit", limitOrderNumber)
            };

            return getObjectsListAsync<IOrder, JsonOrder>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Creates new order for execution using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="symbol">Order asset name.</param>
        /// <param name="quantity">Order quantity.</param>
        /// <param name="side">Order side (buy or sell).</param>
        /// <param name="type">Order type.</param>
        /// <param name="duration">Order duration.</param>
        /// <param name="limitPrice">Order limit price.</param>
        /// <param name="stopPrice">Order stop price.</param>
        /// <param name="clientOrderId">Client order ID.</param>
        /// <param name="extendedHours">Whether or not this order should be allowed to execute during extended hours trading.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only order information object for newly created order.</returns>
        public async Task<IOrder> PostOrderAsync(
            String symbol,
            Int64 quantity,
            OrderSide side,
            OrderType type,
            TimeInForce duration,
            Decimal? limitPrice = null,
            Decimal? stopPrice = null,
            String? clientOrderId = null,
            Boolean? extendedHours = null,
            CancellationToken cancellationToken = default)
        {
            if (clientOrderId != null && 
                clientOrderId.Length > 48)
            {
                clientOrderId = clientOrderId.Substring(0, 48);
            }

            var newOrder = new JsonNewOrder
            {
                Symbol = symbol,
                Quantity = quantity,
                OrderSide = side,
                OrderType = type,
                TimeInForce = duration,
                LimitPrice = limitPrice,
                StopPrice = stopPrice,
                ClientOrderId = clientOrderId,
                ExtendedHours = extendedHours
            };

            await _alpacaRestApiThrottler.WaitToProceed(cancellationToken).ConfigureAwait(false);

            var serializer = new JsonSerializer();
#if NETSTANDARD2_1
#pragma warning disable CA2000 // Dispose objects before losing scope
            await using var stringWriter = new StringWriter();
#pragma warning restore CA2000 // Dispose objects before losing scope
#else
            using var stringWriter = new StringWriter();
#endif

            serializer.Serialize(stringWriter, newOrder);

            using var content = new StringContent(stringWriter.ToString());
            using var response = await _alpacaHttpClient.PostAsync(
                    new Uri("orders", UriKind.RelativeOrAbsolute), content, cancellationToken)
                .ConfigureAwait(false);

            return await deserializeAsync<IOrder, JsonOrder>(response)
                .ConfigureAwait(false);
        }
        
        /// <summary>
        /// Updates existing order using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="orderId">Server side order identifier.</param>
        /// <param name="quantity">Updated order quantity or <c>null</c> if quantity is not changed.</param>
        /// <param name="duration">Updated order duration or <c>null</c> if duration is not changed.</param>
        /// <param name="limitPrice">Updated order limit price or <c>null</c> if limit price is not changed.</param>
        /// <param name="stopPrice">Updated order stop price or <c>null</c> if stop price is not changed.</param>
        /// <param name="clientOrderId">Updated client order ID or <c>null</c> if client order ID is not changed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only order information object for updated order.</returns>
        public async Task<IOrder> PatchOrderAsync(
            Guid orderId,
            Int64? quantity = null,
            TimeInForce? duration = null,
            Decimal? limitPrice = null,
            Decimal? stopPrice = null,
            String? clientOrderId = null,
            CancellationToken cancellationToken = default)
        {
            if (clientOrderId != null && 
                clientOrderId.Length > 48)
            {
                clientOrderId = clientOrderId.Substring(0, 48);
            }

            var changeOrder = new JsonChangeOrder
            {
                Quantity = quantity,
                TimeInForce = duration,
                LimitPrice = limitPrice,
                StopPrice = stopPrice,
                ClientOrderId = clientOrderId,
            };

            await _alpacaRestApiThrottler.WaitToProceed(cancellationToken).ConfigureAwait(false);

            using var request = new HttpRequestMessage(_httpMethodPatch,
                new Uri($"orders/{orderId:D}", UriKind.RelativeOrAbsolute));

            using (var stringWriter = new StringWriter())
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(stringWriter, changeOrder);
                request.Content = new StringContent(stringWriter.ToString());
            }

            using var response = await _alpacaHttpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return await deserializeAsync<IOrder, JsonOrder>(response)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get single order information by client order ID from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="clientOrderId">Client order ID for searching.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only order information object.</returns>
        public Task<IOrder> GetOrderAsync(
            String clientOrderId,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "orders:by_client_order_id",
                Query = new QueryBuilder()
                    .AddParameter("client_order_id", clientOrderId)
            };

            return getSingleObjectAsync<IOrder, JsonOrder>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Get single order information by server order ID from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="orderId">Server order ID for searching.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only order information object.</returns>
        public Task<IOrder> GetOrderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + $"orders/{orderId:D}",
            };

            return getSingleObjectAsync<IOrder, JsonOrder>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Deletes/cancel order on server by server order ID using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="orderId">Server order ID for cancelling.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><c>True</c> if order cancellation was accepted.</returns>
        public async Task<Boolean> DeleteOrderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            await _alpacaRestApiThrottler.WaitToProceed(cancellationToken).ConfigureAwait(false);

            using var response = await _alpacaHttpClient.DeleteAsync(
                    new Uri($"orders/{orderId:D}", UriKind.RelativeOrAbsolute), cancellationToken)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes/cancel all open orders using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>List of order cancellation status objects.</returns>
        public async Task<IReadOnlyList<IOrderActionStatus>> DeleteAllOrdersAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "orders",
            };

            return await deleteObjectsListAsync<IOrderActionStatus, JsonOrderActionStatus>(
                    _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets list of available positions from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of position information objects.</returns>
        public Task<IReadOnlyList<IPosition>> ListPositionsAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "positions",
            };

            return getObjectsListAsync<IPosition, JsonPosition>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets position information by asset name from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="symbol">Position asset name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only position information object.</returns>
        public Task<IPosition> GetPositionAsync(
            String symbol,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + $"positions/{symbol}",
            };

            return getSingleObjectAsync<IPosition, JsonPosition>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Liquidates all open positions at market price using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>List of position cancellation status objects.</returns>
        public async Task<IReadOnlyList<IPositionActionStatus>> DeleteAllPositionsAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "positions",
            };

            return await deleteObjectsListAsync<IPositionActionStatus, JsonPositionActionStatus>(
                    _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Liquidate an open position at market price using Alpaca REST API endpoint.
        /// </summary>
        /// <param name="symbol">Symbol for liquidation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><c>True</c> if position liquidation was accepted.</returns>
        public async Task<Boolean> DeletePositionAsync(
            String symbol,
            CancellationToken cancellationToken = default)
        {
            await _alpacaRestApiThrottler.WaitToProceed(cancellationToken).ConfigureAwait(false);

            using var response = await _alpacaHttpClient.DeleteAsync(
                    new Uri($"positions/{symbol}", UriKind.RelativeOrAbsolute), cancellationToken)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Get current time information from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only clock information object.</returns>
        public Task<IClock> GetClockAsync(
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "clock",
            };

            return getSingleObjectAsync<IClock, JsonClock>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets list of trading days from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="startDateInclusive">Start time for filtering (inclusive).</param>
        /// <param name="endDateInclusive">End time for filtering (inclusive).</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of trading date information object.</returns>
        public Task<IReadOnlyList<ICalendar>> ListCalendarAsync(
            DateTime? startDateInclusive = null,
            DateTime? endDateInclusive = null,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaHttpClient.BaseAddress)
            {
                Path = _alpacaHttpClient.BaseAddress.AbsolutePath + "calendar",
                Query = new QueryBuilder()
                    .AddParameter("start", startDateInclusive, "yyyy-MM-dd")
                    .AddParameter("end", endDateInclusive, "yyyy-MM-dd")
            };

            return getObjectsListAsync<ICalendar, JsonCalendar>(
                _alpacaHttpClient, _alpacaRestApiThrottler, builder, cancellationToken);
        }

        /// <summary>
        /// Gets lookup table of historical daily bars lists for all assets from Alpaca REST API endpoint.
        /// </summary>
        /// <param name="symbols">>Asset names for data retrieval.</param>
        /// <param name="timeFrame">Type of time bars for retrieval.</param>
        /// <param name="areTimesInclusive">
        /// If <c>true</c> - both <paramref name="timeFrom"/> and <paramref name="timeInto"/> parameters are treated as inclusive.
        /// </param>
        /// <param name="timeFrom">Start time for filtering.</param>
        /// <param name="timeInto">End time for filtering.</param>
        /// <param name="limit">Maximal number of daily bars in data response.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Read-only list of daily bars for specified asset.</returns>
        public async Task<IReadOnlyDictionary<String, IEnumerable<IAgg>>> GetBarSetAsync(
            IEnumerable<String> symbols,
            TimeFrame timeFrame,
            Int32? limit = 100,
            Boolean areTimesInclusive = true,
            DateTime? timeFrom = null,
            DateTime? timeInto = null,
            CancellationToken cancellationToken = default)
        {
            var builder = new UriBuilder(_alpacaDataClient.BaseAddress)
            {
                Path = _alpacaDataClient.BaseAddress.AbsolutePath + $"bars/{timeFrame.ToEnumString()}",
                Query = new QueryBuilder()
                    .AddParameter("symbols", String.Join(",", symbols))
                    .AddParameter((areTimesInclusive ? "start" : "after"), timeFrom, "O")
                    .AddParameter((areTimesInclusive ? "end" : "until"), timeInto, "O")
                    .AddParameter("limit", limit)
            };

            var response = await getSingleObjectAsync
                <IReadOnlyDictionary<String, List<JsonBarAgg>>,
                    Dictionary<String, List<JsonBarAgg>>>(
                _alpacaHttpClient, FakeThrottler.Instance, builder, cancellationToken)
                .ConfigureAwait(false);

            return response.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsEnumerable<IAgg>());
        }
    }
}
