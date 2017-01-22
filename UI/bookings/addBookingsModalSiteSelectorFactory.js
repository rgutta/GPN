angular.module('xxx.cinemaHq.app.bookings')
    .factory('addBookingsModalSiteSelectorFactory', ['restRepositoryFactory',
        function (restRepositoryFactory) {
            'use strict';

            return function () {
                var siteSelector = {
                    siteCheckBoxes: [],
                    siteSearchText: '',
                    siteSearchFilter: siteSearchFilter,
                    selectAllSites: selectAllSites,
                    getSelectedSites: getSelectedSites,
                    clearAllSites: clearAllSites,
                    initialize: initialize,
                    toggleSiteCheckBox: toggleSiteCheckBox
                };

                return siteSelector;

                function initialize() {
                    var siteRepository = restRepositoryFactory({
                        entityName: 'site'
                    });

                    return siteRepository.getAll().then(function (sites) {
                        siteSelector.siteCheckBoxes = _.map(sites, function (site) {
                            return {
                                site: site,
                                isChecked: false
                            };
                        });
                        return sites;
                    });
                }

                function siteSearchFilter(siteCheckBox) {
                    var name = siteCheckBox.site.Name;

                    var containsName = _.isTrimmedSubstring(name, siteSelector.siteSearchText);
                    return containsName;
                }

                function selectAllSites() {
                    angular.forEach(siteSelector.siteCheckBoxes, function (siteCheckBox) {
                        if (siteSearchFilter(siteCheckBox)) {
                            siteCheckBox.isChecked = true;
                        }
                    });
                }

                function clearAllSites() {
                    angular.forEach(siteSelector.siteCheckBoxes, function (siteCheckBox) {
                        if (siteSearchFilter(siteCheckBox)) {
                            siteCheckBox.isChecked = false;
                        }
                    });
                }

                function getSelectedSites() {
                    var checkedSiteCheckBoxes = _.filter(siteSelector.siteCheckBoxes, function (siteCheckBox) {
                        return siteCheckBox.isChecked;
                    });

                    return _.map(checkedSiteCheckBoxes, function (siteCheckBox) {
                        return siteCheckBox.site;
                    });
                }

                function toggleSiteCheckBox($event, siteCheckBox) {
                    //Prevent the default browser handling of clicking the checkbox or label so that this
                    //  click handler is not called multiple times
                    $event.preventDefault();

                    siteCheckBox.isChecked = !siteCheckBox.isChecked;
                }
            };
        }]);