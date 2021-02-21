import d3Cloud from 'd3-cloud';
import * as d3 from 'd3';

declare const _TAGS: d3Cloud.Word[];

const width = 800;
const height = 600;

const container = d3.select(".tag-cloud")
    .attr('viewBox', '0 0 800 600');

const cloud = d3Cloud()
    .size([width, height])
    .words(_TAGS)
    .padding(5)
    .rotate(() => 0)
    // .font('Impact')
    .fontSize(function (d) { return d.size; })
    .on("word", ({ size, x, y, rotate, text }) => {
        container.append("text")
            .attr("font-size", size)
            .attr("transform", `translate(${x},${y}) rotate(${rotate})`)
            .text(text)
            .classed("click-only-text", true)
            .classed("word-default", true)
            .on("mouseover", handleMouseOver)
            .on("mouseout", handleMouseOut)
            .on("click", handleClick);

        function handleMouseOver(d: any, i: any) {
            d3.select(this)
                .classed("word-hovered", true)
                .transition(`mouseover-${text}`).duration(300).ease(d3.easeLinear)
                .attr("font-size", size + 2)
                .attr("font-weight", "bold");
        }

        function handleMouseOut(d: any, i: any) {
            d3.select(this)
                .classed("word-hovered", false)
                .interrupt(`mouseover-${text}`)
                .attr("font-size", size);
        }

        function handleClick(d: any, i: any) {
            var e = d3.select(this);
            console.log(e.text());
            // displaySelection.text(`selection="${e.text()}"`);
            e.classed("word-selected", !e.classed("word-selected"));
        }

    });


cloud.start();
