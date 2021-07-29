import React, { useState, useEffect, CSSProperties, useRef, createRef } from "react";
import { Collapse, Progress } from "reactstrap";
import * as signalR from "@aspnet/signalr";
import { Tree, ReactD3TreeItem } from "react-d3-tree";

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

export const useRect = () => {
    const ref = createRef<HTMLDivElement>();
    const [rect, setRect] = useState(getRect(ref ? ref.current : null))

    const handleResize = React.useCallback(() => {
        if (!ref.current) {
            return;
        }

        // Update client rect
        setRect(getRect(ref.current));
    }, [ref]);

    React.useLayoutEffect(() => {
        const element = ref.current
        if (!element) {
            return;
        }

        handleResize();

        if (typeof ResizeObserver === 'function') {
            let resizeObserver: ResizeObserver | null = new ResizeObserver(() => handleResize())
            resizeObserver.observe(element);

            window.req

            return () => {
                if (!resizeObserver) {
                    return;
                }

                resizeObserver.disconnect();
                resizeObserver = null;
            }
        } else {
            // Browser support, remove freely
            window.addEventListener('resize', handleResize);

            return () => {
                window.removeEventListener('resize', handleResize);
            }
        }
    }, [ref.current]);

    return [ref, rect] as [React.RefObject<HTMLDivElement>, DOMRect];
}

function getRect(element: HTMLElement | null) {
    if (!element) {
        return {
            bottom: 0,
            height: 0,
            left: 0,
            right: 0,
            top: 0,
            width: 0,
        }
    }

    return element.getBoundingClientRect()
}

function useSize() {
    const ref = createRef<HTMLDivElement>();
    //const rect = ref.current ? ref.current.getBoundingClientRect() : null;
    const [dimensions, setDimensions] = useState<DOMRect>({} as DOMRect);

    useEffect(() => {
        if(ref.current)
            setDimensions(ref.current.getBoundingClientRect());
    }, [ref.current]);

    return [ref, dimensions] as [React.RefObject<HTMLDivElement>, DOMRect];
}

function SizeMe() {
    const [ref, rect] = useSize();
    console.log('render');
    return (
        <div ref={ref} style={{ width: '100%', height: 'calc(100vh)'}}>
            {rect.width} {rect.height}
        </div>
    );
}

class NodeLabel extends React.PureComponent<any, {}> {
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
    let x = new Array(first);
    for (let i = 0; i < first; i++) {
        x[i] = [];
        for (let c = 0; c < second; c++) {
            x[i].push({} as Item);
        }
    }

    return x;
}

export const Home: React.FunctionComponent = () => {
    let rows: Item[][] = createArray(10, 4);
    let columns: Item[][] = createArray(10, 4);

    let uploadRef: HTMLInputElement | null = null;
    let hubConnection: signalR.HubConnection | null = null;

    const [griddlers, setGriddlers] = useState<Griddler[]>([]);
    const [width, setWidth] = useState(10);
    const [height, setHeight] = useState(10);
    const [depth, setDepth] = useState(4);
    const [sG, setSg] = useState("Bird10x10");
    const [points, setPoints] = useState<Point[]>([]);
    const [dots, setDots] = useState<Point[]>([]);

    const [paths, setPaths] = useState<GriddlerPathGroup[]>([]);
    const [selectedGroup, setSelectedGroup] = useState<GriddlerPathGroup>();

    const [streaming, setStreaming] = useState(false);

    const [showTree, setShowTree] = useState(false);
    const [treeData, setTreeData] = useState<ReactD3TreeItem>({} as ReactD3TreeItem);

    const [shown, setShown] = useState(true);

    useEffect(() => {
        listGriddlers();

        hubConnection = new signalR.HubConnectionBuilder().withUrl("/griddlerhub").build();

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
        var body = {
            rows: rows,
            columns: columns
        };

        fetch("/Home/GetData", {
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

    const onWidthChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        let width = parseInt(event.target.value);
        if (!width)
            width = 1;

        columns = createArray(width, depth);
        setWidth(width);
    }

    const onHeightChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        let height = parseInt(event.target.value);
        if (!height)
            height = 1;

        rows = createArray(height, depth);
        setHeight(height);
    }

    const onDepthChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        let depth = parseInt(event.target.value);
        if (!depth)
            depth = 1;

        columns = createArray(width, depth);
        rows = createArray(height, depth);
        setDepth(depth);
    }

    const onRowChange = (event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) => {
        rows[i][c].value = parseInt(event.target.value);
    }

    const onColumnChange = (event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) => {
        columns[i][c].value = parseInt(event.target.value);
    }

    const onSelectGriddler = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setSg(event.target.value);
    }

    const get = (e: React.MouseEvent<HTMLButtonElement>) => {
        var body = {
            sG: sG
        };

        fetch("/Home/GetGriddler", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                rows = data.r;
                columns = data.c;
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

        fetch("/Home/StreamGriddler", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                rows = data.r;
                columns = data.c;
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
        fetch("/Home/StreamGriddlerNext", {
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
        fetch("/Home/StreamGriddlerPrevious", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                if (data.pt.isDot) {
                    setDots(prevState => {
                        let prevDots =[...prevState];
                        prevDots.splice(prevDots.length - 1, 1);
                        return prevDots;
                    });
                }
                else {
                    setPoints(prevState => {
                        let prevPts = [...prevState];
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

        fetch(url, {
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

        fetch("/Home/GetTreeTest", {
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

        fetch("/Home/GetGriddlerDb?id=" + sG, {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                rows = data.r;
                columns = data.c;

                setWidth(data.w);
                setHeight(data.h);
                setDepth(data.d);
                setPoints(data.pts);
                setDots(data.dots);
                setPaths(data.paths);
            });
    }

    const listGriddlers = () => {
        fetch("/Home/ListGriddlers", {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                setGriddlers(data);
            });
    }

    const listGriddlersDb = () => {
        fetch("/Home/ListGriddlersDb", {
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
        if (uploadRef && uploadRef.files && uploadRef.files.length > 0) {
            let file = uploadRef.files[0];

            let formData = new FormData();
            formData.append("file", file);

            fetch("/Home/UploadImage", {
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

    const renderGriddlerList = () => {
        let html = null;
        let items = [];

        items = griddlers.map(g => <option key={g.name} value={g.name}>{g.name}</option>);

        html = (
            <select className="form-control" onChange={(e) => onSelectGriddler(e)} value={sG}>
                {items}
            </select>
        );

        return html;
    }

    const renderPathList = () => {
        let html = null;

        let items = paths.map(g => {
            let open = true;

            if (selectedGroup && g.group === selectedGroup.group)
                open = true;

            return (
                <div key={g.group}>
                    <p style={{ cursor: "pointer" }} onClick={() => onPathClick(g)}>{g.name}</p>
                    <Collapse isOpen={open}>
                        {g.items.map((m, i) => <p key={i}>({m.x}, {m.y})</p>)}
                    </Collapse>
                </div>
            );
        });

        html = (
            <div style={{
                display: "inline-block",
                position: "relative",
                width: "150px",
                height: "calc(70vh)",
                overflowY: "auto"
            }}>
                {items}
            </div>
        );

        return html;
    }

    let html: JSX.Element;
    let pts: JSX.Element[] = [];
    let xBoxes: JSX.Element[] = [];
    let yBoxes: JSX.Element[] = [];
    let dts: JSX.Element[] = [];
    let grid: JSX.Element[] = [];
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
            let sClass: any = { ...gridStyle };

            if (i % 5 === 0)
                brdL = "1px solid black";

            if (c % 5 === 0)
                brdT = "1px solid black";

            if (selectedGroup
                && selectedGroup.items.some(e => e.x === i && e.y === c))
                sClass.backgroundColor = "red";

            grid.push(
                <div style={{
                    ...sClass, borderLeft: brdL, borderTop: brdT,
                    top: c * squareSize, left: i * squareSize
                }}></div>
            );
        }
    }

    for (let pt of points) {
        if (pt) {
            let sClass: any = { ...style };

            if (selectedGroup
                && selectedGroup.items.some(e => e.x === pt.x && e.y === pt.y))
                sClass.backgroundColor = "red";
            else
                sClass.backgroundColor = pt.colour;

            pts.push(
                <div style={{
                    ...sClass, top: pt.y * squareSize,
                    left: pt.x * squareSize
                }}></div>
            );
        }
    }

    for (let dot of dots) {
        if (dot) {
            let sClass = { ...dotStyle };
            dts.push(
                <div style={{
                    ...sClass, top: (dot.y * squareSize) + 8,
                    left: (dot.x * squareSize) + 8
                }}></div>
            );
        }
    }

    for (let i = 0; i < width; i++) {
        for (let c = 0; c < depth; c++) {
            let left = (depth * squareSize) + (i * squareSize);
            let item = {} as Item;

            if (columns[i] && columns[i][c])
                item = columns[i][c];

            xBoxes.push((
                <input type="text" value={item.value} onChange={(e) => onColumnChange(e, i, c)}
                    style={{
                        width: "20px",
                        height: "20px",
                        position: "absolute",
                        left: left,
                        top: c * squareSize,
                        backgroundColor: item.colour === 'lightgreen' ? "lightgreen" : ""
                    }} />
            )
            );
        }
    }

    for (let i = 0; i < height; i++) {
        for (let c = 0; c < depth; c++) {
            let top = (depth * squareSize) + (i * squareSize);
            let item = {} as Item;

            if (rows[i] && rows[i][c])
                item = rows[i][c];

            yBoxes.push(
                (
                    <input type="text" value={item.value} onChange={(e) => onRowChange(e, i, c)}
                        style={{
                            width: "20px",
                            height: "20px",
                            position: "absolute",
                            left: c * squareSize,
                            top: top,
                            backgroundColor: item.colour === 'lightgreen' ? "lightgreen" : ""
                        }} />
                )
            );
        }
    }

    let square = (depth * squareSize) + "px";
    html = (
        <div>
            <div className="row" style={{ marginTop: "10px", marginBottom: "10px" }}>
                <div className="col-md-6">
                    <div className="input-group">
                        <div className="input-group-btn">
                            <button className="btn btn-primary" onClick={(e) => get(e)}>Get</button>
                        </div>
                        {renderGriddlerList()}
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
                <button className="btn btn-primary btn-sm" onClick={(e) => upload()}>Upload</button>
                <input type="file" ref={el => uploadRef = el}></input>
            </div>
            <div style={{ marginTop: "5px", marginBottom: "10px" }}>
                <input onChange={(e) => onWidthChange(e)} value={width} />
                <input onChange={(e) => onHeightChange(e)} value={height} />
                <input onChange={(e) => onDepthChange(e)} value={depth} />
                <button className="btn btn-primary btn-sm" onClick={(e) => run()}>Run</button>
            </div>
            <div>
                <Progress max={width * height}
                    value={points.length + dots.length} />
            </div>
            {showTree && <div style={{ width: "100%", height: "calc(100vh)" }}>
                <Tree data={treeData} orientation="vertical"
                    transitionDuration={0} allowForeignObjects={true}
                    nodeLabelComponent={{ render: <NodeLabel /> }} />
            </div>}
            <button onClick={() => setShown(!shown)}>Test</button>
            <div className="row">
                {shown && <div className="col-lg-2">
                    {renderPathList()}
                </div>}
                <div className="col" style={{ overflowY: "auto", height: "calc(78vh)" }}>
                    <SizeMe />
                    <div style={{ position: "relative", display: "inline-block", width: square, height: square }}>
                        {xBoxes}{yBoxes}
                    </div>
                    <div style={{ position: "relative", display: "inline-block" }}>{grid}{pts}{dts}</div>
                </div>
            </div>
        </div>
    );

    return html;
}