import "./ServerList.css";
import React from "react";
import { apiURL, appName } from "../../config";
import ServerThumbnail from "./ServerThumbnail/ServerThumbnail";

class ServerList extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      page: 0,
      servers: [],
      total: 0,
      amount: 25,
      all: false,
      randomServer: [],
      ids: [],
    };
    this.randomServer = {};
    this.scrollListener = this.scrollListener.bind(this);
  }

  async scrollListener() {
    const winScroll =
      document.body.scrollTop || document.documentElement.scrollTop;

    const height =
      document.documentElement.scrollHeight -
      document.documentElement.clientHeight;
    if (!this.state.all && winScroll / height > 0.75) {
      await this.addServersToView(this.state.page, this.state.amount);
    }
  }

  async getTotalServers() {
    return await fetch(`${apiURL}/servers/count`).then((json) => json.json());
  }

  async getRandomServer() {
    let json = await fetch(`${apiURL}/servers/random`).then((json) =>
      json.json()
    );
    return json;
  }

  async getServersFromApi(page = 0, amount = 25) {
    if (this.state.all) return [];
    let json = await fetch(`${apiURL}/servers/${page}?amount=${amount}`).then(
      (response) => response.json()
    );
    if (json.length === 0) {
      this.setState({
        all: true,
      });
    }

    return json;
  }

  async addServersToView(page = this.state.page, amount = this.state.amount) {
    const newServers = await this.getServersFromApi(page, amount);
    let oldServers = this.state.servers;
    let i = 0;
    newServers.forEach((server) => {
      if (!this.state.ids.includes(server.serverId)) {
        oldServers.push(server);
        this.state.ids.push(server.serverId);
        i++;
      }
    });
    if (i > 0) {
      this.setState({
        page: this.state.page + 1,
      });
    }
    this.setState({
      servers: oldServers,
    });
  }

  async componentDidMount() {
    window.addEventListener("scroll", this.scrollListener);
    this.setState({
      randomServer: [await this.getRandomServer()],
      total: await this.getTotalServers(),
    });
    this.randomServer = await this.getRandomServer();
    while (
      document.documentElement.clientHeight * 2 >
        document.documentElement.scrollHeight &&
      !this.state.all
    ) {
      await this.addServersToView();
    }
  }

  componentWillUnmount() {
    window.removeEventListener("scroll", this.scrollListener);
  }

  render() {
    let servers = this.state.servers.map((server, index) => {
      return (
        <div key={server.serverId}>
          <ServerThumbnail server={server} index={index} />
        </div>
      );
    });
    let randomServer = this.state.randomServer.map((server) => {
      return (
        <div key={server.serverId}>
          <ServerThumbnail server={server} />
        </div>
      );
    });
    return (
      <div>
        <div className="title minecraft">
          <img src="/logo.png" alt={appName} className="logo" />
          {appName}
        </div>
        <div className="about minecraft">Minecraft Server List</div>
        <div className="h3 minecraft">Random server</div>
        {randomServer}
        <div className="h1 minecraft">
          We have {this.state.total} online servers in our database!
        </div>
        {servers}
      </div>
    );
  }
}

export default ServerList;
