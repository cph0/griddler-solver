import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { CSSProperties } from 'react';
import { Collapse, Progress } from 'reactstrap';
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
    green: boolean;
}

interface Item {
    value: number;
    green: boolean;
}

interface GriddlerPathGroup {
    name: string;
    group: number;
    items: GriddlerPath[];
}

interface GriddlerPath {
    name: string;
    group: number;
    xPos: number;
    yPos: number;
}

interface HomeState {
    griddlers: Griddler[];
    width: number;
    height: number;
    depth: number;
    sG: string;
    points: Point[];
    dots: Point[];
    paths: GriddlerPathGroup[];
    selectedGroup?: GriddlerPathGroup;

    streaming: boolean;

    showTree: boolean;
    treeData: ReactD3TreeItem;
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

export class Home extends React.Component<RouteComponentProps<{}>, HomeState> {
    private rows: Item[][];
    private columns: Item[][];
    private uploadRef: HTMLInputElement | null = null;
    private hubConnection: signalR.HubConnection | null = null;

    constructor(props: any) {
        super(props);

        this.rows = this.createArray(10, 4);
        this.columns = this.createArray(10, 4);

        this.state = {
            griddlers: [],
            width: 10,
            height: 10,
            depth: 4,
            sG: "Bird10x10",
            points: [] as Point[],
            dots: [] as Point[],
            paths: [],
            streaming: false,
            showTree: false,
            treeData: {} as ReactD3TreeItem
        };
    }

    private createArray(first: number, second: number): Item[][] {
        let x = new Array(first);
        for (let i = 0; i < first; i++) {
            x[i] = [];
            for (let c = 0; c < second; c++) {
                x[i].push({} as Item);
            }
        }

        return x;
    }

    componentDidMount() {
        this.listGriddlers();

        this.hubConnection = new signalR.HubConnectionBuilder().withUrl("/griddlerhub").build();

        this.hubConnection.on("SendPoint", (pt) => {
            if (pt.isDot) {
                this.setState(prevState => ({
                    dots: [...prevState.dots, pt]
                }));
            }
            else {
                this.setState(prevState => ({
                    points: [...prevState.points, pt]
                }));
            }
        });

        this.hubConnection.start();
    }

    componentWillUnmount() {
        if(this.hubConnection)
            this.hubConnection.stop();
    }

    private run(event: React.MouseEvent<HTMLButtonElement>) {
        var body = {
            rows: this.rows,
            columns: this.columns
        };

        fetch("/Home/GetData", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.setState({ points: data.pts, dots: data.dots, showTree: false });
            });
    }

    private onWidthChange(event: React.ChangeEvent<HTMLInputElement>) {
        let width = parseInt(event.target.value);
        if (!width)
            width = 1;

        this.columns = this.createArray(width, this.state.depth);

        this.setState({ width: width });
    }

    private onHeightChange(event: React.ChangeEvent<HTMLInputElement>) {
        let height = parseInt(event.target.value);
        if (!height)
            height = 1;

        this.rows = this.createArray(height, this.state.depth);

        this.setState({ height: height });
    }

    private onDepthChange(event: React.ChangeEvent<HTMLInputElement>) {
        let depth = parseInt(event.target.value);
        if (!depth)
            depth = 1;

        this.columns = this.createArray(this.state.width, depth);
        this.rows = this.createArray(this.state.height, depth);

        this.setState({ depth: depth });
    }

    private onRowChange(event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) {
        this.rows[i][c].value = parseInt(event.target.value);
    }

    private onColumnChange(event: React.ChangeEvent<HTMLInputElement>, i: number, c: number) {
        this.columns[i][c].value = parseInt(event.target.value);
    }

    private onSelectGriddler(event: React.ChangeEvent<HTMLSelectElement>) {
        this.setState({ sG: event.target.value });
    }

    private get(e: React.MouseEvent<HTMLButtonElement>) {
        var body = {
            sG: this.state.sG
        };

        fetch("/Home/GetGriddler", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.rows = data.r;
                this.columns = data.c;
                this.setState({
                    width: data.w,
                    height: data.h,
                    depth: data.d,
                    points: data.pts,
                    dots: data.dots,
                    paths: data.paths,
                    showTree: false
                });
            });
    }

    private stream(e: React.MouseEvent<HTMLButtonElement>) {
        const body = {
            sG: this.state.sG
        };

        fetch("/Home/StreamGriddler", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.rows = data.r;
                this.columns = data.c;
                this.setState({
                    width: data.w,
                    height: data.h,
                    depth: data.d,
                    showTree: false,
                    streaming: true,
                    points: [],
                    dots: []
                });
            });
    }

    private next(e: React.MouseEvent<HTMLButtonElement>) {
        fetch("/Home/StreamGriddlerNext", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                if (data.pt.isDot) {
                    this.setState(prevState => ({
                        dots: [...prevState.dots, data.pt]
                    }));
                }
                else {
                    this.setState(prevState => ({
                        points: [...prevState.points, data.pt]
                    }));
                }
            });
    }

    private previous(e: React.MouseEvent<HTMLButtonElement>) {
        fetch("/Home/StreamGriddlerPrevious", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                if (data.pt.isDot) {
                    this.setState(prevState => {
                        let prevDots = [...prevState.dots];
                        prevDots.splice(prevDots.length - 1,1);

                        return {
                            dots: prevDots
                        };
                    });
                }
                else {
                    this.setState(prevState => {
                        let prevPts = [...prevState.points];
                        prevPts.splice(prevPts.length - 1, 1);

                        return {
                            points: prevPts
                        };
                    });
                }
            });
    }

    private playStop(e: React.MouseEvent<HTMLButtonElement>) {
        let url = "/Home/StreamGriddlerPlay";

        if (this.state.streaming)
            url = "/Home/StreamGriddlerStop";

        fetch(url, {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(() => {
                this.setState({streaming: !this.state.streaming});
            });
    }

    private getTree(e: React.MouseEvent<HTMLButtonElement>) {
        const body = {
            sG: this.state.sG
        };

        fetch("/Home/GetTreeTest", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }, body: JSON.stringify(body)
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.setState({
                    showTree: true,
                    treeData: data
                });
            });
    }


    private getDb(e: React.MouseEvent<HTMLButtonElement>) {

        fetch("/Home/GetGriddlerDb?id=" + this.state.sG, {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.rows = data.r;
                this.columns = data.c;
                this.setState({
                    width: data.w,
                    height: data.h,
                    depth: data.d,
                    points: data.pts,
                    dots: data.dots,
                    paths: data.paths
                });
            });
    }

    private listGriddlers() {
        fetch("/Home/ListGriddlers", {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.setState({
                    griddlers: data
                });
            });
    }

    private listGriddlersDb() {
        fetch("/Home/ListGriddlersDb", {
            method: "GET", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.setState({
                    griddlers: data
                });
            });
    }

    private upload() {
        if (this.uploadRef && this.uploadRef.files && this.uploadRef.files.length > 0) {
            let file = this.uploadRef.files[0];

            let formData = new FormData();
            formData.append("file", file);

            fetch("/Home/UploadImage", {
                method: "POST", body: formData
            })
                .then(responce => responce.json() as Promise<any>)
                .then(data => {
                    this.setState({ points: data.pts, dots: data.dots });
                });
        }
    }

    public onPathClick(g: GriddlerPathGroup) {
        let selectedGroup;
        if (!this.state.selectedGroup || this.state.selectedGroup.group != g.group)
            selectedGroup = g;

        this.setState({ selectedGroup });
    }

    public renderGriddlerList() {
        let html = null;
        let items = [];

        items = this.state.griddlers.map(g => <option key={g.name} value={g.name}>{g.name}</option>);

        html = (
            <select className="form-control" onChange={(e) => this.onSelectGriddler(e)} value={this.state.sG}>
                {items}
            </select>
        );

        return html;
    }

    public renderPathList() {
        let html = null;

        let items = this.state.paths.map(g => {
            let open = true;

            if (this.state.selectedGroup && g.group == this.state.selectedGroup.group)
                open = true;

            return (
                <div key={g.group}>
                    <p style={{ cursor: "pointer" }} onClick={() => this.onPathClick(g)}>{g.name}</p>
                    <Collapse isOpen={open}>
                        {g.items.map((m, i) => <p key={i}>({m.xPos}, {m.yPos})</p>)}
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

    public render() {
        let html: JSX.Element;
        let points: JSX.Element[] = [];
        let xBoxes: JSX.Element[] = [];
        let yBoxes: JSX.Element[] = [];
        let dots: JSX.Element[] = [];
        let grid: JSX.Element[] = [];

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

        for (let i = 0; i < this.state.width; i++) {
            for (let c = 0; c < this.state.height; c++) {
                let brdL, brdT: string = "none";
                let sClass = { ...gridStyle };

                if (i % 5 == 0)
                    brdL = "1px solid black";

                if (c % 5 == 0)
                    brdT = "1px solid black";

                if (this.state.selectedGroup
                    && this.state.selectedGroup.items.some(e => e.xPos == i && e.yPos == c))
                    sClass.backgroundColor = "red";

                grid.push(
                    <div style={{ ...sClass, borderLeft: brdL, borderTop: brdT, top: c * 20, left: i * 20 }}></div>
                );
            }
        }

        for (let pt of this.state.points) {
            if (pt) {
                let sClass = { ...style };

                if (this.state.selectedGroup
                    && this.state.selectedGroup.items.some(e => e.xPos * 20 == pt.x && e.yPos * 20 == pt.y))
                    sClass.backgroundColor = "red";
                else if (pt.green)
                    sClass.backgroundColor = "lightgreen";

                points.push(
                    <div style={{ ...sClass, top: pt.y, left: pt.x }}></div>
                );
            }
        }

        for (let dot of this.state.dots) {
            if (dot) {
                let sClass = { ...dotStyle };



                dots.push(
                    <div style={{ ...sClass, top: dot.y + 8, left: dot.x + 8 }}></div>
                );
            }
        }

        for (let i = 0; i < this.state.width; i++) {
            for (let c = 0; c < this.state.depth; c++) {
                let left = (this.state.depth * 20) + (i * 20);
                let item = {} as Item;

                if (this.columns[i][c])
                    item = this.columns[i][c];

                xBoxes.push((
                    <input type="text" value={item.value} onChange={(e) => this.onColumnChange(e, i, c)}
                        style={{
                            width: "20px",
                            height: "20px",
                            position: "absolute",
                            left: left,
                            top: c * 20,
                            backgroundColor: item.green ? "lightgreen" : ""
                        }} />
                )
                );
            }
        }

        for (let i = 0; i < this.state.height; i++) {
            for (let c = 0; c < this.state.depth; c++) {
                let top = (this.state.depth * 20) + (i * 20);
                let item = {} as Item;

                if (this.rows[i][c])
                    item = this.rows[i][c];

                yBoxes.push(
                    (
                        <input type="text" value={item.value} onChange={(e) => this.onRowChange(e, i, c)}
                            style={{
                                width: "20px",
                                height: "20px",
                                position: "absolute",
                                left: c * 20,
                                top: top,
                                backgroundColor: item.green ? "lightgreen" : ""
                            }} />
                    )
                );
            }
        }

        let square = (this.state.depth * 20) + "px";

        html = (
            <div>
                <div className="row" style={{ marginTop: "10px", marginBottom: "10px" }}>
                    <div className="col-md-6">
                        <div className="input-group">
                            <div className="input-group-btn">
                                <button className="btn btn-primary" onClick={(e) => this.get(e)}>Get</button>
                            </div>
                            {this.renderGriddlerList()}
                            <div className="input-group-btn">
                                <button className="btn btn-primary" onClick={(e) => this.getTree(e)}>Tree</button>
                                <button className="btn btn-primary" onClick={(e) => this.stream(e)}>Stream</button>
                                <button className="btn btn-primary" onClick={(e) => this.previous(e)}>Back</button>
                                <button className="btn btn-primary" onClick={(e) => this.playStop(e)}>
                                    {this.state.streaming ? "Pause" : "Play"}
                                </button>
                                <button className="btn btn-primary" onClick={(e) => this.next(e)}>Forward</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div style={{ display: "none" }}>
                    <button className="btn btn-primary btn-sm" onClick={(e) => this.upload()}>Upload</button>
                    <input type="file" ref={el => this.uploadRef = el}></input>
                </div>
                <div style={{ marginTop: "5px", marginBottom: "10px" }}>
                    <input onChange={(e) => this.onWidthChange(e)} value={this.state.width} />
                    <input onChange={(e) => this.onHeightChange(e)} value={this.state.height} />
                    <input onChange={(e) => this.onDepthChange(e)} value={this.state.depth} />
                    <button className="btn btn-primary btn-sm" onClick={(e) => this.run(e)}>Run</button>
                </div>
                <div>
                    <Progress max={this.state.width * this.state.height}
                        value={this.state.points.length + this.state.dots.length} />
                </div>
                {this.state.showTree && <div style={{ width: "100%", height: "calc(100vh)" }}>
                    <Tree data={this.state.treeData} orientation="vertical"
                        transitionDuration={0} allowForeignObjects={true}
                        nodeLabelComponent={{ render: <NodeLabel /> }} />
                </div>}
                <div className="row">
                    <div className="col-lg-2">
                        {this.renderPathList()}
                    </div>
                    <div className="col">
                        <div style={{ position: "relative", display: "inline-block", width: square, height: square }}>
                            {xBoxes}{yBoxes}
                        </div>
                        <div style={{ position: "relative", display: "inline-block" }}>{grid}{points}{dots}</div>
                    </div>
                </div>
            </div>

        );

        return html;
    }
}
