const nodetest = require("node:test");
const supertest = require("supertest");
const comments = require("./videoCommentsHardcoded");

const apiURL = "http://localhost:8005/api/video";

const commentAPI = supertest(apiURL);

nodetest.describe("the description", (describeOptions) => {
  nodetest.test("input some video comments", async (testOptions) => {
    // console.log(comments);
    comments.forEach(async (cm) => {
      await commentAPI.post("/2/comment")
        .set('Accept', 'application/json')
        .send(cm)
        .expect(200)
        ;
    });
  });
});
