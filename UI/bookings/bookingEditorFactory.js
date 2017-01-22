angular.module('xxx.cinemaHq.app.bookings')
    .service('bookingEditorFactory', ['restRepositoryFactory', 'baseEditorFactory', 
        function (restRepositoryFactory, baseEditorFactory) {
            'use strict';

            return function (params) {
                var $scope = params.$scope;

                var restRepository = restRepositoryFactory({
                    entityName: 'booking'
                });

                return baseEditorFactory({
                    restRepository: restRepository,
                    $scope: $scope,
                    buttonConfig: ["save", "cancel", "remove"]
                });
            };
        }]);