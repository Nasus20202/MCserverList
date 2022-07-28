import './ServerList.css';
import React from 'react';
import config from '../../config.json';
import ServerThumbnail from './ServerThumbnail/ServerThumbnail';

class ServerList extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            page:0,
            servers: [],
            amount: 1,
            all: false,
            ids: []
        }
        this.scrollListener = this.scrollListener.bind(this);
    }

    async scrollListener(){
        const winScroll =
        document.body.scrollTop || document.documentElement.scrollTop
    
      const height =
        document.documentElement.scrollHeight -
        document.documentElement.clientHeight
      if(!this.state.all && winScroll / height > 0.75){
        await this.addServersToView(this.state.page+1, this.state.amount);
      }
    }

    async getServersFromApi(page = 0, amount = 25){
        if(this.state.all)
            return [];
        let json = await  fetch(
            `${config.url}servers/${page}?amount=${amount}`)
            .then(response => response.json())
            console.log(json)
        if(json.length === 0)
            this.setState({
                all: true
            })
        return json;
    }

    async addServersToView(page = 0, amount = 25){
        const newServers = await this.getServersFromApi(page, amount);
        let oldServers = this.state.servers;
        newServers.forEach(server => {
            if(!this.state.ids.includes(server.serverId)){
                oldServers.push(server);
                this.state.ids.push(server.serverId);
                this.setState({
                    page: this.state.page+1
                });
            }
        });
        this.setState({
            servers: oldServers
        });
    }


    async componentDidMount(){
        window.addEventListener('scroll', this.scrollListener);
        await this.addServersToView(0, this.state.amount);
        if(document.documentElement.clientHeight === document.documentElement.scrollHeight){
            await this.addServersToView(this.state.page+1, this.state.amount);
            this.setState({
                page: this.state.page+1
            });
        }
    }

    componentWillUnmount(){
        window.removeEventListener('scroll', this.scrollListener);
    }

    render(){
        let servers = this.state.servers.map((server, index) => {
            return <div key={server.serverId}><ServerThumbnail server={server}/></div>
            });
        return(
            <div>
                <div className='h1 minecraft'>
                    Server List
                </div>
                {servers}

            </div>
        )
    }
}

export default ServerList;