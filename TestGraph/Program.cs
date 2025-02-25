﻿// See https://aka.ms/new-console-template for more information


using TestGraph.LomisTry;


var nodeIn = new Node(0, "IN");
var nodeN1 = new Node(1, "N1");
var nodeN2 = new Node(2, "N2");
var nodeOut = new Node(3, "OUT");

var light1 = new Light("L1", 529);
var light2 = new Light("L2", 529);
var resistor1 = new Resistor("R1", 200);
var resistor2 = new Resistor("R2", 150);
var resistor3 = new Resistor("R3", 100);
var switch1 = new Resistor("S", 0);

var branchInN1Left = new BranchGraph(nodeIn, nodeN1, new List<IElecComponent>() { light1 });
var branchInN1Right = new BranchGraph(nodeIn, nodeN1, new List<IElecComponent>() { resistor2 });
var branchN1N2 = new BranchGraph(nodeN1, nodeN2, new List<IElecComponent>() { switch1 });
var branchInN2 = new BranchGraph(nodeIn, nodeN2, new List<IElecComponent>() { resistor1, resistor3 });
var branchN2Out = new BranchGraph(nodeN2, nodeOut, new List<IElecComponent>() { light2 });

var graphInN1 = new ParallelGraph(nodeIn, nodeN1, new List<Graph>() { branchInN1Left, branchInN1Right });
var graphInN2Right = new SeriesGraph(nodeIn, nodeN2, new List<Graph>() { graphInN1, branchN1N2 });
var graphInN2 = new ParallelGraph(nodeIn, nodeN2, new List<Graph>() { branchInN2, graphInN2Right });
var main = new SeriesGraph(nodeIn, nodeOut, new List<Graph>(){graphInN2, branchN2Out});

main.LaunchEletrons(230, 16);