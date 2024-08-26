import "./Tags.css";
import React from "react";
import Badge from "react-bootstrap/Badge";

function Tags(props) {
  const tags = props.tags.map((tag, index) => {
    return (
      <Badge bg="secondary" className="tag" key={tag.name + index.toString()}>
        {tag.name}
      </Badge>
    );
  });
  return <div>{tags}</div>;
}

export default Tags;
