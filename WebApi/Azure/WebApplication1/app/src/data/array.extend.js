//Adds to array prototype to add filter and aggregate options for dashboard

Array.prototype.groupByAndSum = function (groupBy, unit) {
    var group = _.groupBy(this, groupBy);
    return _.map(group, function (g, key) {
        return {
            group: key,
            unit: pmdCalcHelpers.sum(g, unit)
        };
    });
};
