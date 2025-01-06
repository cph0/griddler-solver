import React, { useState, useEffect, CSSProperties, JSX, useRef } from "react";
import { Collapse, Progress } from "reactstrap";
import * as signalR from "@aspnet/signalr";
import { RawNodeDatum, Tree } from "react-d3-tree";
import { BarChart, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Bar, ResponsiveContainer } from "recharts";
//import manifest from "../manifest.json" with { type: "json" };

const api = "https://localhost:5001";

interface Griddler {
    id: number;
    name: string;
}

interface Point {
    isDot: boolean;
    x: number;
    y: number;
    colour: string;
}

interface Item {
    value: number;
    colour: string;
}

interface GriddlerPathGroup {
    name: string;
    group: number;
    items: GriddlerPath[];
}

interface GriddlerPath {
    name: string;
    group: number;
    x: number;
    y: number;
}

class NodeLabel extends React.PureComponent<any, object> {
    render() {
        const { nodeData } = this.props
        return (
            <div style={{ border: "1px solid black", padding: "5px" }}>
                <span>{nodeData.name}</span>
            </div>
        )
    }
}

const createArray = (first: number, second: number): Item[][] => {
    const x = new Array(first);
    for (let i = 0; i < first; i++) {
        x[i] = [];
        for (let c = 0; c < second; c++) {
            x[i].push({} as Item);
        }
    }

    return x;
}

function useTest(){}

function test(){
    return () => useTest();
}

interface ChartData {
    name: string;
    value: number;
}

export const Home = () => {
    //let rows: Item[][] = createArray(10, 4);
    //let columns: Item[][] = createArray(10, 4);


    const uploadRef = useRef<HTMLInputElement>(null);
    const [griddlers, setGriddlers] = useState<Griddler[]>([]);
    const [width, setWidth] = useState(10);
    const [height, setHeight] = useState(10);
    const [depth, setDepth] = useState(4);
    const [rows, setRows] = useState([]);
    const [columns, setColumns] = useState([]);
    const [sG, setSg] = useState("Bird10x10");
    const [points, setPoints] = useState<Point[]>([]);
    const [dots, setDots] = useState<Point[]>([]);

    const [paths, setPaths] = useState<GriddlerPathGroup[]>([]);
    const [selectedGroup, setSelectedGroup] = useState<GriddlerPathGroup>();

    const [streaming, setStreaming] = useState(false);

    const [showTree, setShowTree] = useState(false);
    const [treeData, setTreeData] = useState<RawNodeDatum>({} as RawNodeDatum);

    useEffect(() => {
        const listGriddlers = () => {
            fetch(`${api}/Home/ListGriddlers`, {
                method: "GET", headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(responce => responce.json() as Promise<any>)
                .then(data => {
                    setGriddlers(data);
                });
        }
        listGriddlers();

        const hubConnection = new signalR.HubConnectionBuilder().withUrl("/griddlerhub").build();

        hubConnection.on("SendPoint", (pt) => {
            if (pt.isDot) {
                setDots(prevState => [...prevState, pt]);
            }
            else {
                setPoints(prevState => [...prevState, pt]);
            }
        });

        hubConnection.start();

        return () => {
            if (hubConnection)
                hubConnection.stop();
        };
    }, []);

    const run = () => {
        const body = {
            rows: rows,
            columns: columns
        };

        fetch(`${api}/Home/GetData`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setPoints(data.pts);
                setDots(data.dots);
                setShowTree(false);
            });
    }

    //const onWidthChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    //    let width = parseInt(event.target.value);
    //    if (!width)
    //        width = 1;

    //    columns = createArray(width, depth);
    //    setWidth(width);
    //}

    //const onHeightChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    //    let height = parseInt(event.target.value);
    //    if (!height)
    //        height = 1;

    //    rows = createArray(height, depth);
    //    setHeight(height);
    //}

    //const onDepthChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    //    let depth = parseInt(event.target.value);
    //    if (!depth)
    //        depth = 1;

    //    columns = createArray(width, depth);
    //    rows = createArray(height, depth);
    //    setDepth(depth);
    //}

    //const onRowChange = (event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) => {
    //    rows[i][c].value = parseInt(event.target.value);
    //}

    //const onColumnChange = (event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) => {
    //    columns[i][c].value = parseInt(event.target.value);
    //}

    const onSelectGriddler = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setSg(event.target.value);
    }

    const get = (e: React.MouseEvent<HTMLButtonElement>) => {
        const body = {
            sG: sG
        };

        fetch(`${api}/Home/GetGriddler`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setRows(data.r);
                setColumns(data.c);
                setWidth(data.w);
                setHeight(data.h);
                setDepth(data.d);
                setPoints(data.pts);
                setDots(data.dots);
                setPaths(data.paths);
                setShowTree(false);
            });
    }

    const stream = (e: React.MouseEvent<HTMLButtonElement>) => {
        const body = {
            sG: sG
        };

        fetch(`${api}/Home/StreamGriddler`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setRows(data.r);
                setColumns(data.c);
                setWidth(data.w);
                setHeight(data.h);
                setDepth(data.d);
                setShowTree(false);
                setStreaming(true);
                setPoints([]);
                setDots([]);
            });
    }

    const next = (e: React.MouseEvent<HTMLButtonElement>) => {
        fetch(`${api}/Home/StreamGriddlerNext`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                if (data.pt.isDot) {
                    setDots(prevState => [...prevState, data.pt]);
                }
                else {
                    setPoints(prevState => [...prevState, data.pt]);
                }
            });
    }

    const previous = (e: React.MouseEvent<HTMLButtonElement>) => {
        fetch(`${api}/Home/StreamGriddlerPrevious`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                if (data.pt.isDot) {
                    setDots(prevState => {
                        const prevDots =[...prevState];
                        prevDots.splice(prevDots.length - 1, 1);
                        return prevDots;
                    });
                }
                else {
                    setPoints(prevState => {
                        const prevPts = [...prevState];
                        prevPts.splice(prevPts.length - 1, 1);
                        return prevPts;
                    });
                }
            });
    }

    const playStop = (e: React.MouseEvent<HTMLButtonElement>) => {
        let url = "/Home/StreamGriddlerPlay";

        if (streaming)
            url = "/Home/StreamGriddlerStop";

        fetch(`${api}${url}`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(() => {
                setStreaming(!streaming);
            });
    }

    const getTree = (e: React.MouseEvent<HTMLButtonElement>) => {
        const body = {
            sG: sG
        };

        fetch(`${api}/Home/GetTreeTest`, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setShowTree(true);
                setTreeData(data);
            });
    }


    const getDb = (e: React.MouseEvent<HTMLButtonElement>) => {

        fetch(`${api}/Home/GetGriddlerDb?id=` + sG, {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setRows(data.r);
                setColumns(data.c);

                setWidth(data.w);
                setHeight(data.h);
                setDepth(data.d);
                setPoints(data.pts);
                setDots(data.dots);
                setPaths(data.paths);
            });
    }    

    const listGriddlersDb = () => {
        fetch(`${api}/Home/ListGriddlersDb`, {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setGriddlers(data);
            });
    }

    const upload = () => {
        if (uploadRef?.current && uploadRef.current.files && uploadRef.current.files.length > 0) {
            const file = uploadRef.current.files[0];

            const formData = new FormData();
            formData.append("file", file);

            fetch(`${api}/Home/UploadImage`, {
                method: "POST", body: formData
            })
                .then(responce => responce.json() as Promise<any>)
                .then(data => {
                    setPoints(data.pts);
                    setDots(data.dots);
                });
        }
    }

    const onPathClick = (g: GriddlerPathGroup) => {
        let selected;
        if (!selectedGroup || selectedGroup.group != g.group)
            selected = g;

        setSelectedGroup(selected);
    }

    const pts: JSX.Element[] = [];
    const xBoxes: JSX.Element[] = [];
    const yBoxes: JSX.Element[] = [];
    const dts: JSX.Element[] = [];
    const grid: JSX.Element[] = [];
    const squareSize = 20;

    const style: CSSProperties = {
        position: "absolute",
        backgroundColor: "black",
        width: "20px",
        height: "20px"
    };

    const dotStyle: CSSProperties = {
        position: "absolute",
        backgroundColor: "black",
        width: "4px",
        height: "4px"
    };

    const gridStyle: CSSProperties = {
        position: "absolute",
        borderRight: "1px solid black",
        borderBottom: "1px solid black",
        width: "20px",
        height: "20px"
    };

    for (let i = 0; i < width; i++) {
        for (let c = 0; c < height; c++) {
             let brdL, brdT: string = "none";
             const sClass = { 
                ...gridStyle,
                //backgroundColor: selectedGroup?.items.some(e => e.x == i && e.y == c) ? "red" : "white"
             };

            if (i % 5 == 0)
                brdL = "1px solid black";

            if (c % 5 == 0)
                brdT = "1px solid black";

            grid.push(
                <div key={`${i}_${c}`}
                    style={{
                        ...sClass, borderLeft: brdL, borderTop: brdT,
                        top: c * squareSize, left: i * squareSize
                    }}></div>
            );
        }
    }

    for (const pt of points) {
        if (pt) {
            const sClass = { ...style };

            if (selectedGroup
                && selectedGroup.items.some(e => e.x == pt.x && e.y == pt.y))
                sClass.backgroundColor = "red";
            else 
                sClass.backgroundColor = pt.colour;

            pts.push(
                <div key={`${pt.x}_${pt.y}`}
                    style={{
                        ...sClass, top: pt.y * squareSize,
                        left: pt.x * squareSize
                    }}></div>
            );
        }
    }

    for (const dot of dots) {
        if (dot) {
            const sClass = { ...dotStyle };

            dts.push(
                <div key={`${dot.x}_${dot.y}`}
                    style={{
                        ...sClass, top: (dot.y * squareSize) + 8,
                        left: (dot.x * squareSize) + 8
                    }}></div>
            );
        }
    }

    for (let i = 0; i < width; i++) {
        for (let c = 0; c < depth; c++) {
            const left = (depth * squareSize) + (i * squareSize);
            let item = {} as Item;

            if (columns[i] && columns[i][c])
                item = columns[i][c];

            xBoxes.push((
                <input
                    key={`${i}_${c}`}
                    type="text"
                    value={item.value}
                    onChange={() => { }}
                    style={{
                        width: "20px",
                        height: "20px",
                        position: "absolute",
                        left: left,
                        top: c * squareSize,
                        backgroundColor: item.colour !== 'black' ? item.colour : ""
                    }} />
            )
            );
        }
    }

    for (let i = 0; i < height; i++) {
        for (let c = 0; c < depth; c++) {
            const top = (depth * squareSize) + (i * squareSize);
            let item = {} as Item;

            if (rows[i] && rows[i][c])
                item = rows[i][c];

            yBoxes.push(
                (
                    <input
                        key={`${i}_${c}`}
                        type="text"
                        value={item.value}
                        onChange={() => { }}
                        style={{
                            width: "20px",
                            height: "20px",
                            position: "absolute",
                            left: c * squareSize,
                            top: top,
                            backgroundColor: item.colour !== 'black' ? item.colour : ""
                        }} />
                )
            );
        }
    }

    const square = (depth * squareSize) + "px";

    return (
        <div>
            <div className="row" style={{ marginTop: "10px", marginBottom: "10px" }}>
                <div className="col-md-6">
                    <div className="input-group">
                        <div className="input-group-btn">
                            <button className="btn btn-primary" onClick={(e) => get(e)}>Get</button>
                        </div>
                        <select className="form-control" onChange={(e) => onSelectGriddler(e)} value={sG}>
                            {griddlers.map(g => <option key={g.name} value={g.name}>{g.name}</option>)}
                        </select>
                        <div className="input-group-btn">
                            <button className="btn btn-primary" onClick={(e) => getTree(e)}>Tree</button>
                            <button className="btn btn-primary" onClick={(e) => stream(e)}>Stream</button>
                            <button className="btn btn-primary" onClick={(e) => previous(e)}>Back</button>
                            <button className="btn btn-primary" onClick={(e) => playStop(e)}>
                                {streaming ? "Pause" : "Play"}
                            </button>
                            <button className="btn btn-primary" onClick={(e) => next(e)}>Forward</button>
                        </div>
                    </div>
                </div>
            </div>
            <div style={{ display: "none" }}>
                <button className="btn btn-primary btn-sm" onClick={() => upload()}>Upload</button>
                <input type="file" ref={uploadRef}></input>
            </div>
            <div>
                <Progress max={width * height}
                    value={points.length + dots.length} />
            </div>
            {showTree && <div style={{ width: "100%", height: "calc(100vh)" }}>
                <Tree data={treeData} orientation="vertical"
                    transitionDuration={0}
                    renderCustomNodeElement={props=><NodeLabel {...props} />} />
            </div>}
            <div className="row">
                <div className="col-lg-2">                
                    <div style={{
                        display: "inline-block",
                        position: "relative",
                        width: "150px",
                        height: "calc(70vh)",
                        overflowY: "auto"
                    }}>
                        {paths.map(g => {
                            let open = true;

                            if (selectedGroup && g.group == selectedGroup.group)
                                open = true;

                            return (
                                <div key={g.group}>
                                    <p style={{ cursor: "pointer" }} onClick={() => onPathClick(g)}>{g.name}</p>
                                    <Collapse isOpen={open}>
                                        {g.items.map((m, i) => <p key={i}>({m.x}, {m.y})</p>)}
                                    </Collapse>
                                </div>
                            );
                        })}
                    </div>
                </div>
                <div className="col" style={{ overflowY: "auto", height: "calc(89vh)" }}>
                    <div style={{ position: "relative", display: "inline-block", width: square, height: square }}>
                        {xBoxes}{yBoxes}
                    </div>
                    <div style={{ position: "relative", display: "inline-block" }}>{grid}{pts}{dts}</div>
                </div>
            </div>
        </div>
    );
}