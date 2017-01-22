angular.module('xxx.cinemaHq.app.bookings')
    .factory('addBookingsModalFeatureSelectorFactory', ['restRepositoryFactory',
        function (restRepositoryFactory) {
            'use strict';

            return function () {
                var featureSelector = {
                    features: [],
                    selectedFeatures: [],
                    selectedFeature: undefined,
                    getFeatureNameAndYear: getFeatureNameAndYear,
                    selectFeature: selectFeature,
                    unselectFeature: unselectFeature,
                    unselectAllFeatures: unselectAllFeatures,
                    initialize: initialize
                };

                return featureSelector;

                function initialize() {
                    var featureRepository = restRepositoryFactory({
                        entityName: 'feature'
                    });

                    return featureRepository.getAll().then(function (features) {
                        var featuresWithReleaseYear = _.map(features, function(feature) {
                            var copy = angular.copy(feature);
                            copy.ReleaseYear = getReleaseYear(feature);
                            return copy;
                        });

                        featureSelector.features = featuresWithReleaseYear;
                        return featuresWithReleaseYear;
                    });
                }

                function getFeatureNameAndYear(feature) {
                    if (feature === undefined || feature === null) {
                        return undefined;
                    }

                    var name = feature.Name;
                    var releaseYear = feature.ReleaseYear;
                    var output = name;
                    output += releaseYear === undefined ? '' : ' (' + releaseYear + ')';
                    return output;
                }

                function getReleaseYear(feature) {
                    var releaseDate = feature.ReleaseDate;
                    return releaseDate === null || releaseDate === undefined ?
                        undefined :
                        moment(releaseDate).year().toString();
                }

                function selectFeature(feature) {
                    var selectedFeatures = featureSelector.selectedFeatures;
                    var featureAlreadySelected = _.some(selectedFeatures, function (selectedFeature) {
                        return selectedFeature.Id === feature.Id;
                    });

                    featureSelector.selectedFeature = undefined;

                    if (featureAlreadySelected) {
                        return;
                    }

                    featureSelector.selectedFeatures.push(feature);
                }

                function unselectFeature(index) {
                    featureSelector.selectedFeatures.splice(index, 1);
                }

                function unselectAllFeatures() {
                    featureSelector.selectedFeatures = [];
                }
            };
        }]);