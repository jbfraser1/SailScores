﻿"use strict";

(function () {

    var margin = 30;
    var margin = { top: 30, right: 30, bottom: 70, left: 60 },
        chartWidth = 460 - margin.left - margin.right,
        chartHeight = 300 - margin.top - margin.bottom;

    var charts = document.getElementsByClassName("season-chart");
    for (var i = 0; i < charts.length; i++) {
        drawChart("#" + charts[i].id,
            charts[i].dataset.competitorId,
            charts[i].dataset.seasonName)
    }


    function drawChart(elementId, competitorId, seasonName ) {
        var chartElementId = elementId;
        var dataPath = "/competitor/chart?competitorId=" + competitorId +
            "&seasonName=" + seasonName;
        if (typeof(d3) != "undefined" && d3 != null) {
            d3.json(dataPath).then(processChartData);
        }


        function processChartData(data) {
            if (data === null || data.length === 1) {
                return;
            }
            var counts = data.map(r => r.count || 0);
            let minCount = 0;
            let maxCount = Math.max(...counts);


            //var svgElement = d3.select(chartElementId)
            //    .attr("width", chartWidth)
            //    .attr("height", chartHeight)
            //    //.call(responsivefy)
                ;
            var svgElement = d3.select(chartElementId)
                .attr("width", chartWidth + margin.left + margin.right)
                .attr("height", chartHeight + margin.top + margin.bottom)
                .append("g")
                .attr("transform",
                    "translate(" + margin.left + "," + margin.top + ")");

            var xScale = d3.scaleBand()
                .range([0, chartWidth])
                .domain(data.map(function (d) { return d.place || d.code; }))
                .padding(0.3);

            svgElement.append("g")
                .attr("transform", "translate(0," + chartHeight + ")")
                .call(d3.axisBottom(xScale))
                .selectAll("text")
                .attr("transform", "translate(-10,0)rotate(-45)")
                .style("text-anchor", "end");


            var yScale = d3.scaleLinear()
                .domain([minCount, maxCount])
                .range([chartHeight, 0]);

            svgElement.append("g").call(d3.axisLeft(yScale));

            svgElement.append("text")
                .attr("text-anchor", "middle")
                .attr("x", chartWidth / 2)
                .attr("y", chartHeight + margin.top + 15)
                .text("Place Finished");

            // Y axis label:
            svgElement.append("text")
                .attr("text-anchor", "middle")
                .attr("transform", "rotate(-90)")
                .attr("y", -margin.left + 30)
                .attr("x", -chartHeight / 2)
                .text("# of Races")

            // Bars
            svgElement.selectAll("mybar")
                .data(data)
                .enter()
                .append("rect")
                .attr("x", function (d) { return xScale(d.place || d.code); })
                .attr("y", function (d) { return yScale(d.count); })
                .attr("width", xScale.bandwidth())
                .attr("height", function (d) { return chartHeight - yScale(d.count); })
                .attr("fill", "#265180")


            //function onMouseOver(d) {
            //    compId = d.competitorId || d.id;
            //    svgElement
            //        .selectAll("path.compLine")
            //        .attr("opacity", .4);
            //    svgElement
            //        .selectAll("path[data-compId='" + compId + "']")
            //        .attr("stroke-width", 4)
            //        .attr("opacity", 1);
            //    svgElement
            //        .selectAll("g.legendEntry")
            //        .attr("opacity", .4);
            //    svgElement
            //        .selectAll("g.legendEntry[data-compId='" + compId + "']")
            //        .attr("opacity", 1);
            //    svgElement
            //        .selectAll("g.legendEntry[data-compId='" + compId + "'] rect")
            //        .attr("stroke", (c) => color(c.id));
            //    svgElement.selectAll("circle")
            //        .attr("opacity", .4);
            //    svgElement
            //        .selectAll("circle[data-compId='" + compId + "']")
            //        .attr("opacity", 1);

            //}
            //function onMouseOut(d) {
            //    svgElement
            //        .selectAll("path.compLine")
            //        .attr("stroke-width", 1.5);
            //    svgElement
            //        .selectAll("path.compLine")
            //        .attr("opacity", 1);
            //    svgElement
            //        .selectAll("g.legendEntry rect")
            //        .attr("stroke", "none");
            //    tooltipGroup
            //        .attr("opacity", 0)
            //        .attr("transform", "translate(" + chartOverallWidth + ","
            //            + chartOverallHeight + ")");

            //    svgElement
            //        .selectAll("path.compLine")
            //        .attr("opacity", 1);
            //    svgElement
            //        .selectAll("g.legendEntry")
            //        .attr("opacity", 1);
            //    svgElement.selectAll("circle")
            //        .attr("opacity", 1);
            //}



            //lineData = d3.line()
            //    .x(d => xScale(getDate(d, data)))
            //    .y(d => getY(d, data));


            //tooltipGroup = svgElement
            //    .append("g")
            //    .attr("opacity", 0);
            //tooltipGroup.append("rect")
            //    .attr("width", 120)
            //    .attr("height", legendLineHeight * 3)
            //    .attr("fill", "white")
            //    .attr("fill-opacity", ".7");
            //tooltipGroup.append("text")
            //    .attr('x', 5)
            //    .attr("y", legendLineHeight - 5)
            //    .style("font-size", "11px");

        }
    }

    function responsivefy(svg) {
        // get container + svg aspect ratio
        //var container = d3.select(svg.node().parentNode),
        //    width = parseInt(svg.style("width")),
        //    height = parseInt(svg.style("height")),
        //    aspect = width / height;

        //// add viewBox and preserveAspectRatio properties,
        //// and call resize so that svg resizes on inital page load
        //svg.attr("viewBox", "0 0 " + width + " " + height)
        //    .attr("preserveAspectRatio", "xMinYMid")
        //    .call(resize);

        //// to register multiple listeners for same event type, 
        //// you need to add namespace, i.e., 'click.foo'
        //// necessary if you call invoke this function for multiple svgs
        //// api docs: https://github.com/mbostock/d3/wiki/Selections#on
        //d3.select(window).on("resize." + container.attr("id"), resize);

        //// get width of container and resize svg to fit it
        //function resize() {
        //    var targetWidth = parseInt(container.style("width"));
        //    if (targetWidth <= 100) {// assume percent
        //        targetWidth = width;
        //    }
        //    svg.attr("width", targetWidth);
        //    svg.attr("height", Math.round(targetWidth / aspect));
        //}
    }

    function processChartData(data) {
        if (data === null) {
            return;
        }
        var counts = data.map(r => r.count || 0);
        let minCount = 0;
        let maxCount = Math.max(...counts);


        var svgElement = d3.select(chartElementId)
            .attr("width", chartWidth)
            .attr("height", chartHeight)
            //.call(responsivefy)
            ;

        var xScale = d3.scaleBand()
            .range([0, chartWidth])
            .domain(data.map(function (d) { return (""+d.place) || d.code; }))
            .padding(0.3);

        svgElement.append("g")
            .attr("transform", "translate(0," + chartHeight + ")")
            .call(d3.axisBottom(xScale))
            .selectAll("text")
            .attr("transform", "translate(-10,0)rotate(-45)")
            .style("text-anchor", "end");


        var yScale = d3.scaleLinear()
            .domain([minCount, maxCount])
            .range([chartHeight, 0]);

        svgElement.append("g").call(d3.axisLeft(yScale));

        // Bars
        svgElement.selectAll("mybar")
            .data(data)
            .enter()
            .append("rect")
            .attr("x", function (d) { return xScale(("" + d.place) || d.code); })
            .attr("y", function (d) { return yScale(d.count); })
            .attr("width", xScale.bandwidth())
            .attr("height", function (d) { return chartHeight - yScale(d.count); })
            .attr("fill", "#69b3a2")


        //function onMouseOver(d) {
        //    compId = d.competitorId || d.id;
        //    svgElement
        //        .selectAll("path.compLine")
        //        .attr("opacity", .4);
        //    svgElement
        //        .selectAll("path[data-compId='" + compId + "']")
        //        .attr("stroke-width", 4)
        //        .attr("opacity", 1);
        //    svgElement
        //        .selectAll("g.legendEntry")
        //        .attr("opacity", .4);
        //    svgElement
        //        .selectAll("g.legendEntry[data-compId='" + compId + "']")
        //        .attr("opacity", 1);
        //    svgElement
        //        .selectAll("g.legendEntry[data-compId='" + compId + "'] rect")
        //        .attr("stroke", (c) => color(c.id));
        //    svgElement.selectAll("circle")
        //        .attr("opacity", .4);
        //    svgElement
        //        .selectAll("circle[data-compId='" + compId + "']")
        //        .attr("opacity", 1);

        //}
        //function onMouseOut(d) {
        //    svgElement
        //        .selectAll("path.compLine")
        //        .attr("stroke-width", 1.5);
        //    svgElement
        //        .selectAll("path.compLine")
        //        .attr("opacity", 1);
        //    svgElement
        //        .selectAll("g.legendEntry rect")
        //        .attr("stroke", "none");
        //    tooltipGroup
        //        .attr("opacity", 0)
        //        .attr("transform", "translate(" + chartOverallWidth + ","
        //            + chartOverallHeight + ")");

        //    svgElement
        //        .selectAll("path.compLine")
        //        .attr("opacity", 1);
        //    svgElement
        //        .selectAll("g.legendEntry")
        //        .attr("opacity", 1);
        //    svgElement.selectAll("circle")
        //        .attr("opacity", 1);
        //}



        //lineData = d3.line()
        //    .x(d => xScale(getDate(d, data)))
        //    .y(d => getY(d, data));


        //tooltipGroup = svgElement
        //    .append("g")
        //    .attr("opacity", 0);
        //tooltipGroup.append("rect")
        //    .attr("width", 120)
        //    .attr("height", legendLineHeight * 3)
        //    .attr("fill", "white")
        //    .attr("fill-opacity", ".7");
        //tooltipGroup.append("text")
        //    .attr('x', 5)
        //    .attr("y", legendLineHeight - 5)
        //    .style("font-size", "11px");

    }
    return {
        drawChart: drawChart
    };

})();