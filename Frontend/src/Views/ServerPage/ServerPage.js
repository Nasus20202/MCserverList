import "./ServerPage.css";
import React from "react";
import { useParams } from "react-router-dom";
import ServerDetails from "./ServerDetails/ServerDetails";

function ServerPage() {
  const { serverId } = useParams();
  return <ServerDetails serverId={serverId} />;
}

export default ServerPage;
