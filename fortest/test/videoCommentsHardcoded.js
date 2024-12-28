const single = {
  "userID": 1,
  "comment": "StringComment",
};

const comments = [];

for (let index = 1; index <= 80; index++) {
  const cm = { ...single };
  cm.comment += index;
  comments.push(cm);
}

module.exports = comments;
