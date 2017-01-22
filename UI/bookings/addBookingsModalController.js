angular.module('xxx.cinemaHq.app.bookings')
    .controller('addBookingsModalController', [
        '$scope', '$modal', '$modalInstance', 'restRepositoryFactory', 'addBookingsModalFeatureSelectorFactory',
            'addBookingsModalSiteSelectorFactory', 'errorAlerterFactory', '$q',
        function ($scope, $modal, $modalInstance, restRepositoryFactory, addBookingsModalFeatureSelectorFactory,
            addBookingsModalSiteSelectorFactory, errorAlerterFactory, $q) {
            'use strict';

            //Due to a bug in angular-bootstrap, the $scope here is not bound properly to the view scope
            //unless you wrap it in an object. Therefore, everything is wrapped in the modal object.
            var modal = {
                startDate: undefined,
                endDate: undefined,
                term: undefined,
                submitBookings: submitBookings,
                openAddFeatureModal: openAddFeatureModal,
                isAllowedToSubmitBookings: isAllowedToSubmitBookings,
                isFirstInputFocused: false,
                focus: focus
            };

            angular.extend($scope, {
                modal: modal,
                featureSelector: addBookingsModalFeatureSelectorFactory(),
                siteSelector: addBookingsModalSiteSelectorFactory(),
                errorAlerter: errorAlerterFactory()
            });

            initialize();
            focus();

            function initialize() {
                return $q.all([
                    $scope.featureSelector.initialize(),
                    $scope.siteSelector.initialize()
                ]);
            }

            function submitBookings() {
                var bookingRepository = restRepositoryFactory({
                    entityName: 'booking'
                });

                var selectedSites = $scope.siteSelector.getSelectedSites();
                var selectedFeatures = $scope.featureSelector.selectedFeatures;

                var joined = _.cartesianProduct(selectedSites, selectedFeatures);
                var bookings = _.map(joined, function (joinedElement) {
                    var selectedSite = joinedElement[0];
                    var selectedFeature = joinedElement[1];

                    return {
                        Site: {
                            Id: selectedSite.Id
                        },
                        Feature: {
                            Id: selectedFeature.Id
                        },
                        Term: modal.term,
                        Start: modal.startDate,
                        InclusiveEnd: modal.endDate
                    };
                });

                var addParams = {
                    list: true
                };
                bookingRepository.add(bookings, addParams).then(function () {
                    $modalInstance.close();
                },  function(error) {
                    $scope.errorAlerter.add(error);
                });
            }

            function openAddFeatureModal() {
                var featureModalInstance = $modal.open({
                    templateUrl: 'js/features/featureEditorModal.html',
                    controller: 'FeatureEditorModalController',
                    size: 'lg'
                });
                featureModalInstance.result.then(function (newFeature) {
                    $scope.featureSelector.features.push(newFeature);
                    $scope.featureSelector.selectFeature(newFeature);
                });
            }

            function isAllowedToSubmitBookings() {
                return $scope.featureSelector.selectedFeatures.length > 0 &&
                    $scope.siteSelector.getSelectedSites().length > 0;
            }

            function focus() {
                $scope.featureSelector.isFirstInputFocused = true;
            }
        }
    ]);