
<template>
    <h2>Subreddit: {{ name }}</h2>
    <div class="box">
        <div class="tbl tbl-small">
            <h3>Posts By User</h3>
            <table>
                <thead>
                    <tr>
                        <th>User</th>
                        <th># of Posts</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in responseData.usersWithPosts" :key="user.author">
                        <td><a :href="`https://www.reddit.com/u/${user.author}`" target="_blank">{{ user.author }}</a></td>
                        <td>{{ user.posts }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="tbl tbl-big">
            <h3>Top Posts (By Upvotes)</h3>
            <table>
                <thead>
                    <tr>
                        <th>Post Title</th>
                        <th># of Upvotes</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="post in responseData.postsByUpvotes" :key="post.name">
                        <td><a :href="`https://www.reddit.com/r/${post.subreddit}/comments/${post.id}`" target="_blank">{{ post.title }}</a></td>
                        <td>{{ post.ups }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<style lang="scss">
.box {
    display: flex;

    .tbl {
        border: 1px solid black;
    }

    .tbl-big {
        flex: 1 0 auto;
    }

    .tbl-small {
        flex: 0 1 auto;
    }
}
</style>

<script setup lang="ts">
import { reactive } from "vue";
const props = defineProps({
    name: String
});
type RedditPost = {
    author: string,
    title: string,
    subreddit: string,
    ups: number,
    score: number,
    name: string,
    id: string
}

type ResponseObj = {
    postsByUpvotes: RedditPost[],
    usersWithPosts: {author: string, posts: number}[],
}

let responseData = reactive<ResponseObj>({
    postsByUpvotes: [],
    usersWithPosts: []
});

setInterval(() => {
    fetch(`reddit/${props.name}`)
        .then(r => r.json())
        .then((json: ResponseObj) => {
            responseData.postsByUpvotes = json.postsByUpvotes;
            responseData.usersWithPosts = json.usersWithPosts;
        });
}, 5000);
</script>