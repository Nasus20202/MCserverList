import React from "react";
import NewServer from "./NewServer";
import { useNavigate } from "react-router-dom";

function NewServerHook() {
  const navigate = useNavigate();
  return <NewServer navigate={navigate} />;
}

export default NewServerHook;
