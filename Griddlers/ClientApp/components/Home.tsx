import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { CSSProperties } from 'react';
import { Collapse } from 'reactstrap';

interface Griddler {
    id: number;
    name: string;
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
    sortOrder: number;
}

interface HomeState {
    griddlers: Griddler[];
    width: number;
    height: number;
    depth: number;
    sG: number;
    points: { x: number, y: number, green: boolean }[];
    dots: { x: number, y: number }[];
    paths: GriddlerPathGroup[];
    selectedGroup?: GriddlerPathGroup;
}

export class Home extends React.Component<RouteComponentProps<{}>, HomeState> {
    private rows: Item[][];
    private columns: Item[][];
    private uploadRef: HTMLInputElement | null = null;

    constructor(props: any) {
        super(props);

        this.rows = this.createArray(10, 4);
        this.columns = this.createArray(10, 4);

        this.state = {
            griddlers: [],
            width: 10,
            height: 10,
            depth: 4,
            sG: 1,
            points: [] as { x: number, y: number, green: boolean }[],
            dots: [] as { x: number, y: number }[],
            paths: []
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
                this.setState({ points: data.pts, dots: data.dots });
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
        this.setState({ sG: parseInt(event.target.value, 10) });
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
                    dots: data.dots
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
        this.setState({ selectedGroup: g });
    }

    public renderStaticList() {
        let html = null;

        html = (
            <select onChange={(e) => this.onSelectGriddler(e)} value={this.state.sG}>
                <option value="Bird10x10">Bird10x10</option>
                <option value="Man10x10">Man10x10</option>
                <option value="Rabbit10x10">Rabbit10x10</option>
                <option value="Bug10x9">Bug10x9</option>
                <option value="Snail10x10">Snail10x10</option>
                <option value="Leaf10x10">Leaf10x10</option>
                <option value="Notes10x10">Notes10x10</option>
                <option value="Tree10x10">Tree10x10</option>
                <option value="TV10x10">TV10x10</option>
                <option value="Heart10x10">Heart10x10</option>
                <option value="Mouse10x10">Mouse10x10</option>
                <option value="Face10x10">Face10x10</option>
                <option value="Coffee10x10">Coffee10x10</option>
                <option value="IceCream11x20">IceCream11x20</option>
                <option value="ChessKnight13x20">ChessKnight13x20</option>
                <option value="Matryoshka13x20">Matryoshka13x20</option>
                <option value="StripedFish20x14">StripedFish20x14</option>
                <option value="Necklace14x20">Necklace14x20</option>
                <option value="Pineapple14x20">Pineapple14x20</option>
                <option value="Cheburashka20x14">Cheburashka20x14</option>
                <option value="Apple16x15">Apple16x15</option>
                <option value="Jar17x16">Jar17x16</option>
                <option value="Sakura17x15">Sakura17x15</option>
                <option value="Goose18x15">Goose18x15</option>
                <option value="PolarBear19x14">PolarBear19x14</option>
                <option value="Flower15x15">Flower15x15</option>
                <option value="Yoga15x15">Yoga15x15</option>
                <option value="Swan15x15">Swan15x15</option>
                <option value="Mouse15x15">Mouse15x15</option>
                <option value="Turtles15x15">Turtles15x15</option>
                <option value="Clock15x15">Clock15x15</option>
                <option value="Moose15x15">Moose15x15</option>
                <option value="Pelican15x15">Pelican15x15</option>
                <option value="Girafe15x15">Girafe15x15</option>
                <option value="Ski15x15">Ski15x15</option>
                <option value="Shared15x15">Shared15x15</option>
                <option value="Amphora15x15">Amphora15x15</option>
                <option value="Itzy15x15">Itzy15x15</option>
                <option value="Ostrich15x15">Ostrich15x15</option>
                <option value="Bye15x15">Bye15x15</option>
                <option value="Sparrow15x15">Sparrow15x15</option>
                <option value="InTheGym15x15">InTheGym15x15</option>
                <option value="Cook15x15">Cook15x15</option>
                <option value="Bulb15x15">Bulb15x15</option>
                <option value="Celebration15x15">Celebration15x15</option>
                <option value="Spider15x15">Spider15x15</option>
                <option value="Wet15x15">Wet15x15</option>
                <option value="Wicked15x15">Wicked15x15</option>
                <option value="Tropical15x15">Tropical15x15</option>
                <option value="Creature15x15">Creature15x15</option>
                <option value="Little15x15">Little15x15</option>
                <option value="Liar15x15">Liar15x15</option>
                <option value="Yawn15x15">Yawn15x15</option>
                <option value="Bird15x15">Bird15x15</option>
                <option value="Beer15x15">Beer15x15</option>
                <option value="Sail15x17">Sail15x17</option>
                <option value="GirlInSunglasses15x17">GirlInSunglasses15x17</option>
                <option value="JollyRoger15x20">JollyRoger15x20</option>
                <option value="Gun15x20">Gun15x20</option>
                <option value="Glasses15x20">Glasses15x20</option>
                <option value="Doggie15x20">Doggie15x20</option>
                <option value="Boot15x20">Boot15x20</option>
                <option value="Jeep15x20">Jeep15x20</option>
                <option value="Goldfish15x20">Goldfish15x20</option>
                <option value="Fencer15x20">Fencer15x20</option>
                <option value="Diplodocus15x20">Diplodocus15x20</option>
                <option value="HereDaisy15x20">HereDaisy15x20</option>
                <option value="CountryLodge15x20">CountryLodge15x20</option>
                <option value="Pig15x20">Pig15x20</option>
                <option value="Pumpkin15x20">Pumpkin15x20</option>
                <option value="NightLight15x20">NightLight15x20</option>
                <option value="Vampire15x20">Vampire15x20</option>
                <option value="Bonfire15x20">Bonfire15x20</option>
                <option value="Poodle15x20">Poodle15x20</option>
                <option value="Teapot17x14">Teapot17x14</option>
                <option value="Fishes17x15">Fishes17x15</option>
                <option value="Mortarboard17x15">Mortarboard17x15</option>
                <option value="Sushi19x13">Sushi19x13</option>
                <option value="Ninja22x12">Ninja22x12</option>
                <option value="Landfall20x15">Landfall20x15</option>
                <option value="Centaur20x15">Centaur20x15</option>
                <option value="Tomahawk20x20">Tomahawk20x20</option>
                <option value="Dino20x20">Dino20x20</option>
                <option value="Butterfly20x20">Butterfly20x20</option>
                <option value="Teapot20x20">Teapot20x20</option>
                <option value="Kitty20x20">Kitty20x20</option>
                <option value="Bus20x20">Bus20x20</option>
                <option value="Peacock20x20">Peacock20x20</option>
                <option value="Portable20x20">Portable20x20</option>
                <option value="Dragon20x20">Dragon20x20</option>
                <option value="Worzel20x20">Worzel20x20</option>
                <option value="Mask20x20">Mask20x20</option>
                <option value="Highlights20x20">Highlights20x20</option>
                <option value="FastFood20x20">FastFood20x20</option>
                <option value="Beg20x20">Beg20x20</option>
                <option value="WhatsThat20x20">WhatsThat20x20</option>
                <option value="Volley20x20">Volley20x20</option>
                <option value="Budgie20x20">Budgie20x20</option>
                <option value="Lion20x20">Lion20x20</option>
                <option value="Swing20x20">Swing20x20</option>
                <option value="Phone20x20">Phone20x20</option>
                <option value="Flower20x20">Flower20x20</option>
                <option value="Crash20x20">Crash20x20</option>
                <option value="House20x20">House20x20</option>
                <option value="Man20x20">Man20x20</option>
                <option value="Girl20x20">Girl20x20</option>
                <option value="Knight20x20">Knight20x20</option>
                <option value="Ninja20x20">Ninja20x20</option>
                <option value="Chick20x20">Chick20x20</option>
                <option value="Edible20x20">Edible20x20</option>
                <option value="Little20x20">Little20x20</option>
                <option value="Large20x20">Large20x20</option>
                <option value="Agile20x30">Agile20x30</option>
                <option value="Victory20x30">Victory20x30</option>
                <option value="Lion25x25">Lion25x25</option>
                <option value="Train25x25">Train25x25</option>
                <option value="Raccoon25x25">Raccoon25x25</option>
                <option value="UFO25x25">UFO25x25</option>
                <option value="Santa25x25">Santa25x25</option>
                <option value="Pegasus25x25">Pegasus25x25</option>
                <option value="Tea25x25">Tea25x25</option>
                <option value="Crab25x25">Crab25x25</option>
                <option value="Zen25x25">Zen25x25</option>
                <option value="Cock25x25">Cock25x25</option>
                <option value="Goblin25x25">Goblin25x25</option>
                <option value="Koala25x25">Koala25x25</option>
                <option value="Acorns25x25">Acorns25x25</option>
                <option value="LittleDevil25x25">LittleDevil25x25</option>
                <option value="Girl25x25">Girl25x25</option>
                <option value="Boy25x25">Boy25x25</option>
                <option value="Elephant30x30">Elephant30x30</option>
                <option value="Lovers30x30">Lovers30x30</option>
                <option value="Woman30x30">Woman30x30</option>
                <option value="Pumpkin30x30">Pumpkin30x30</option>
                <option value="Joker30x30">Joker30x30</option>
                <option value="Pinapple30x30">Pinapple30x30</option>
                <option value="Chaplin30x30">Chaplin30x30</option>
                <option value="Cobra30x30">Cobra30x30</option>
                <option value="KingKong30x30">KingKong30x30</option>
            </select>
        );

        return html;
    }

    public renderGriddlerList() {
        let html = null;
        let items = [];

        items = this.state.griddlers.map(g => <option key={g.id} value={g.id}>{g.name}</option>);

        html = (
            <select onChange={(e) => this.onSelectGriddler(e)} value={this.state.sG}>
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
                        {g.items.map(m => <p key={m.sortOrder}>({m.xPos}, {m.yPos})</p>)}
                    </Collapse>
                </div>
            );
        });

        html = (
            <div style={{ display: "inline-block", position: "relative", width: "150px" }}>
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

                if (i % 5 == 0)
                    brdL = "1px solid black";

                if (c % 5 == 0)
                    brdT = "1px solid black";

                grid.push(
                    <div style={{ ...gridStyle, borderLeft: brdL, borderTop: brdT, top: c * 20, left: i * 20 }}></div>
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

                if (this.state.selectedGroup
                    && this.state.selectedGroup.items.some(e => e.xPos * 20 == dot.x && e.yPos * 20 == dot.y))
                    sClass.backgroundColor = "red";

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
                                backgroundColor: item.green ? "lightgreen": ""
                            }} />
                    )
                );
            }
        }

        let square = (this.state.depth * 20) + "px";

        html = (
            <div>
                <div style={{ marginTop: "10px", marginBottom: "10px" }}>
                    <button onClick={(e) => this.getDb(e)}>Get</button>
                    {this.renderGriddlerList()}
                    <button onClick={(e) => this.upload()}>Upload</button>
                    <input type="file" ref={el => this.uploadRef = el}></input>
                </div>
                <div style={{ marginTop: "5px", marginBottom: "10px" }}>
                    <input onChange={(e) => this.onWidthChange(e)} value={this.state.width} />
                    <input onChange={(e) => this.onHeightChange(e)} value={this.state.height} />
                    <input onChange={(e) => this.onDepthChange(e)} value={this.state.depth} />
                    <button onClick={(e) => this.run(e)}>Run</button>
                </div>
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
