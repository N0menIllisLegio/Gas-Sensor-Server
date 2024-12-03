import {
    ApexChart,
    ApexXAxis,
    ApexDataLabels,
    ApexTooltip,
    ApexStroke,
} from "ng-apexcharts";

export type ChartOptions = {
    chart: ApexChart;
    stroke: ApexStroke;
    xaxis: ApexXAxis;
    tooltip: ApexTooltip;
    dataLabels: ApexDataLabels;
};
