import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import {
    BarChart, CartesianGrid, XAxis, YAxis,
    Tooltip, Legend, Bar, ResponsiveContainer
} from "recharts";

interface ChartData {
    name: string;
    value: number;
}

interface ActionsState {
    data: ChartData[];
}

export class Actions extends React.Component<RouteComponentProps<{}>, ActionsState> {

    constructor(props: any) {
        super(props);

        this.state = {
            data: []
        };
    }

    componentDidMount() {
        this.get();
    }

    private get() {

        fetch("/Home/GetActionsChart", {
            method: "POST", headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(responce => responce.json() as Promise<any>)
            .then(data => {
                this.setState({
                    data
                });
            });
    }


    render() {
        let html;

        html = (
            <div>
                <div className="row" style={{ marginTop: "10px", marginBottom: "10px" }}>
                    <div className="col-md-6">
                        <div>
                            <button className="btn btn-primary" onClick={() => this.get()}>Get</button>
                        </div>
                    </div>
                </div>
                <div>
                    <ResponsiveContainer width="100%" height={1000}>
                        <BarChart
                            layout="vertical"
                            data={this.state.data}
                            margin={{
                                top: 5, right: 30, left: 20, bottom: 5,
                            }}
                        >
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis type="number" label="Num of Griddlers" />
                            <YAxis type="category" dataKey="name" label="Action"/>
                            <Tooltip />
                            <Legend />
                            <Bar dataKey="used" fill="#8884d8" />
                            <Bar dataKey="required" fill="#82ca9d" />
                        </BarChart>
                    </ResponsiveContainer>
                </div>
            </div>
        );

        return html;
    }
}