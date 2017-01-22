angular.module('xxx.cinemaHq.app.bookings')
    .service('bookingGridFactory', ['restRepositoryFactory', 'baseGridFactory', 'clockService',
        function (restRepositoryFactory, baseGridFactory, clockService) {
            'use strict';

            return function (params) {
                var $scope = params.$scope;

                var restRepository = restRepositoryFactory({
                    entityName: 'booking'
                });

                var grid = baseGridFactory({
                    restRepository: restRepository,
                    $scope: $scope
                });

                var today = clockService.getTodayCalendarDate();

                var dateFilters = {
                    startDateFilter: today,
                    endDateFilter: undefined
                };
                grid.dateFilters = dateFilters;

                $scope.$watch(function() {
                    return {
                        startDateFilter: dateFilters.startDateFilter,
                        endDateFilter: dateFilters.endDateFilter
                    };
                }, refreshGridFromWatch, true);

                var baseRefresh = grid.refresh;
                angular.extend(grid, {
                    refresh: refreshGrid,
                    isFilterCleared: false,
                    clearFilters: clearFilters
                });

                return grid;

                function clearFilters() {
                    grid.isFilterCleared = true;
                }

                function refreshGridFromWatch(newValue, oldValue) {
                    var isDatePickerWatchInitializing = newValue === oldValue;
                    if (isDatePickerWatchInitializing) {
                        return;
                    }

                    refreshGrid();
                }

                function refreshGrid() {
                    return baseRefresh({
                        start: dateFilters.startDateFilter || '',
                        inclusiveEnd: dateFilters.endDateFilter || ''
                    }).then(function (entities) {
                        grid.clearFilters();
                        grid.entities = sortBookings(entities);
                    });
                }

                function sortBookings(entities) {
                    var ordered = _.sortBy(entities, function (entity) {
                        return entity.Site.Name;
                    });

                    ordered = _.sortBy(ordered, function (entity) {
                        return entity.Feature.Name;
                    });

                    ordered = _.sortBy(ordered, function (entity) {
                        return entity.Start;
                    });

                    return ordered;
                }
            };
        }]);