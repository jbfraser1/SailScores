﻿/// <reference path="../node_modules/devbridge-autocomplete/typings/jquery-autocomplete/jquery.autocomplete.d.ts" />
import { competitorDto, scoreCodeDto } from "./interfaces/server";

import { Guid } from "./guid";
import * as dragula from "dragula";

let competitors: competitorDto[];
declare var scoreCodes: scoreCodeDto[];
const noCodeString = "No Code";

dragula([document.getElementById('results')])
    .on('drop', function () {
        calculatePlaces();
    });

function checkEnter(e: KeyboardEvent) {
    const ev = e || event;
    var txtArea = /textarea/i.test((ev.srcElement).tagName);
    return txtArea || (e.keyCode || e.which || e.charCode || 0) !== 13;
}

document.querySelector('form').onkeypress = checkEnter;
$("#raceform").submit(function (e) {
    e.preventDefault();
    var form = this as HTMLFormElement;
    addScoresFieldsToForm(form);
    form.submit();
    removeScoresFieldsFromForm(form);
});

export function loadFleet() {
    let clubId = ($("#clubId").val() as string);
    let fleetId = ($("#fleetId").val() as string);
    getCompetitors(clubId, fleetId);
}
var c: number = 0;
function addNewCompetitor(competitor: competitorDto) {
    var resultDiv = document.getElementById("results");
    var compTemplate = document.getElementById("competitorTemplate");
    var compListItem = (compTemplate.cloneNode(true) as HTMLLIElement);
    compListItem.id = competitor.id.toString();
    compListItem.setAttribute("data-competitorId", competitor.id.toString());
    var span = compListItem.getElementsByClassName("competitor-name")[0];
    span.appendChild(document.createTextNode(competitor.name));

    span = compListItem.getElementsByClassName("sail-number")[0];
    span.appendChild(document.createTextNode(competitor.sailNumber));

    span = compListItem.getElementsByClassName("race-place")[0];
    span.appendChild(document.createTextNode(c.toString()));

    compListItem.style.display = "";
    resultDiv.appendChild(compListItem);
    calculatePlaces();
}

function addScoresFieldsToForm(form: HTMLFormElement) {
    var resultList = document.getElementById("results");
    var resultItems = resultList.getElementsByTagName("li");

    for (var i = 1; i < resultItems.length; i++) {
        const listIndex = (i - 1).toString();
        var input = document.createElement("input");
        input.type = "hidden";
        input.name = "Scores\[" + listIndex + "\].competitorId";
        input.value = resultItems[i].getAttribute("data-competitorId");
        form.appendChild(input);

        input = document.createElement("input");
        input.type = "hidden";
        input.name = "Scores\[" + listIndex + "\].place";
        if (shouldCompKeepScore(resultItems[i])) {
            input.value = resultItems[i].getAttribute("data-place");
        } else {
        }
        form.appendChild(input);

        input = document.createElement("input");
        input.type = "hidden";
        input.name = "Scores\[" + listIndex + "\].code";
        input.value = getCompetitorCode(resultItems[i]);
        form.appendChild(input);

    }
}

function removeScoresFieldsFromForm(form: HTMLFormElement) {
    form.find("[name^=Scores]").remove();
}
function calculatePlaces() {
    var resultList = document.getElementById("results");
    var resultItems = resultList.getElementsByTagName("li");
    var scoreCount = 1;
    for (var i = 1, len = resultItems.length; i < len; i++) {
        var span = resultItems[i].getElementsByClassName("race-place")[0];
        resultItems[i].setAttribute("data-place", i.toString());
        if (span.id != "competitorTemplate") {
            if (shouldCompKeepScore(resultItems[i])) {
                span.textContent = (scoreCount++).toString();
            } else {
                span.textContent = getCompetitorCode(resultItems[i]);
            }
        }
    }
}

function competitorIsInResults(comp: competitorDto) {
    var resultList = document.getElementById("results");
    var resultItems = resultList.getElementsByTagName("li");
    for (var i = 0, len = resultItems.length; i < len; i++) {
        if (resultItems[i].getAttribute("data-competitorId")
            === comp.id.toString()) {
            return true;
        }
    }
    return false;
}

function getSuggestions(): AutocompleteSuggestion[] {
    const competitorSuggestions: AutocompleteSuggestion[] = [];
    console.debug("checking for comps in results");
    allCompetitors.forEach(c => {
        if (!competitorIsInResults(c)) {
            competitorSuggestions.push(
                {
                    value: c.sailNumber + " - " + c.name,
                    data: c
                });
        }
    });
    return competitorSuggestions;
}

var allCompetitors: competitorDto[];
var competitorSuggestions: AutocompleteSuggestion[];
function getCompetitors(clubId: string, fleetId: string) {

    $.getJSON("/api/Competitors",
        {
            clubId: clubId,
            fleetId: fleetId
        },
        function (data: competitorDto[]) {
            allCompetitors = data;
            initializeAutoComplete();
        });
}

var autoCompleteSetup: boolean = false;
function initializeAutoComplete() {
    competitorSuggestions = getSuggestions();
    if (autoCompleteSetup) {
        $('#newCompetitor').autocomplete().dispose();
    }
    $('#newCompetitor').autocomplete({

        lookup: competitorSuggestions,
        onSelect: function (suggestion: AutocompleteSuggestion) {
            addNewCompetitor(suggestion.data as competitorDto);

            $('#newCompetitor').val("");
            initializeAutoComplete();
        },
        autoSelectFirst: true,
        triggerSelectOnValidInput: false,
        noCache: true
    });
    autoCompleteSetup = true;
}

function getCompetitorCode(compListItem: HTMLLIElement) {
    const codeText = (compListItem.getElementsByClassName("select-code")[0] as HTMLSelectElement).value;
    if (codeText === noCodeString) {
        return null;
    }
    return codeText;
}

function shouldCompKeepScore(compListItem: HTMLLIElement): boolean {
    const codeText =
        (compListItem.getElementsByClassName("select-code")[0] as HTMLSelectElement)
        .value;
    if (codeText === noCodeString) {
        return true;
    }
    const fullCodeObj = scoreCodes.filter(s => s.text === codeText);
    return !!(fullCodeObj[0].countAsCompetitor);
}

