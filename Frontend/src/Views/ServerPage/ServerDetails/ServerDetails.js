import "./ServerDetails.css";
import React from "react";
import { apiURL } from "../../../config";
import Tags from "../../../Components/Tags/Tags";
import Motd from "../../../Components/Motd/Motd";

class ServerDetails extends React.Component {
  constructor(props) {
    super(props);
    this.serverId = this.props.serverId;
    this.state = {
      server: { tags: [] },
    };
    this.copyUrlToClipboard = this.copyUrlToClipboard.bind(this);
  }

  async getServer() {
    const data = await fetch(`${apiURL}/servers/${this.serverId}`);
    const json = await data.json();
    if (data.status !== 200) json["name"] = "Server not found";
    return json;
  }

  async componentDidMount() {
    this.setState({
      server: await this.getServer(),
    });
  }

  copyUrlToClipboard() {
    const url = this.state.server.url;
    navigator.clipboard.writeText(url);
  }

  render() {
    let tags;
    let motd;
    if (this.state.server.tags === undefined) tags = <div>Loading...</div>;
    else tags = <Tags className="detail-tags" tags={this.state.server.tags} />;
    if (this.state.server.motd === undefined) motd = <div>Loading...</div>;
    else
      motd = (
        <div>
          <Motd motd={this.state.server.motd} />
        </div>
      );
    return (
      <div>
        <div className="title minecraft">
          <img
            src={apiURL + this.state.server.image}
            alt={this.state.server.serverId}
            className="logo serverDetailLogo"
          />
          {this.state.server.name}
        </div>
        <div
          className="minecraft url-container"
          onClick={this.copyUrlToClipboard}
        >
          Url: {this.state.server.url}
          <div style={{ fontSize: "0.5em" }}>Click to copy</div>
        </div>
        <div className="detail-info minecraft">
          <span>
            Players: {this.state.server.players}/{this.state.server.maxPlayers}{" "}
          </span>
          <span>
            Version: {this.state.server.version}{" "}
            {this.state.server.premium ? "Premium" : "Non-Premium"}
          </span>
        </div>
        {tags}
        <div className="detailsMotd">{motd}</div>
        <div className="detailsAbout">{this.state.server.about}</div>
      </div>
    );
  }
}

export default ServerDetails;
