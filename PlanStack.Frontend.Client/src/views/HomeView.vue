<template>
  <v-container class="py-10" fluid>
      <div class="hero-text">
        <h1 class="text-h4 font-weight-bold text-grey-darken-3">Planstack</h1>
        <h1>Welcome</h1>
      </div>
    <div class="hero-video-container">
      <video
        ref="videoA"
        :src="videoSrcA"
        class="hero-video"
        :class="{ 'visible': isVideoAVisible, 'hidden': !isVideoAVisible }"
        autoplay
        muted
        playsinline
        @ended="onVideoEnded('A')"
      ></video>
      <video
        ref="videoB"
        :src="videoSrcB"
        class="hero-video"
        :class="{ 'visible': !isVideoAVisible, 'hidden': isVideoAVisible }"
        autoplay
        muted
        playsinline
        @ended="onVideoEnded('B')"
      ></video>
    </div>
  </v-container>
</template>
<script setup>
import { ref, onMounted, nextTick } from 'vue';

const videoFiles = [
  '/videos/planstackvid1.mp4',
  '/videos/planstackvid2.mp4',
  '/videos/planstackvid3.mp4',
];

const videoA = ref(null);
const videoB = ref(null);
const isVideoAVisible = ref(true);
const videoSrcA = ref(videoFiles[Math.floor(Math.random() * videoFiles.length)]);
const videoSrcB = ref('');
let currentVideoIndex = videoFiles.indexOf(videoSrcA.value);

function getNextVideoIndex(excludeIndex) {
  let nextIndex;
  do {
    nextIndex = Math.floor(Math.random() * videoFiles.length);
  } while (nextIndex === excludeIndex && videoFiles.length > 1);
  return nextIndex;
}

function onVideoEnded(which) {
  // Prepare the next video source
  const nextIndex = getNextVideoIndex(currentVideoIndex);
  if (which === 'A') {
    videoSrcB.value = videoFiles[nextIndex];
    nextTick(() => {
      if (videoB.value) {
        videoB.value.load();
        videoB.value.play();
      }
      // Crossfade
      isVideoAVisible.value = false;
      setTimeout(() => {
        currentVideoIndex = nextIndex;
        // Prepare next for A
        videoSrcA.value = videoFiles[getNextVideoIndex(currentVideoIndex)];
      }, 800); // match CSS transition
    });
  } else {
    videoSrcA.value = videoFiles[nextIndex];
    nextTick(() => {
      if (videoA.value) {
        videoA.value.load();
        videoA.value.play();
      }
      // Crossfade
      isVideoAVisible.value = true;
      setTimeout(() => {
        currentVideoIndex = nextIndex;
        // Prepare next for B
        videoSrcB.value = videoFiles[getNextVideoIndex(currentVideoIndex)];
      }, 800);
    });
  }
}

onMounted(() => {
  if (videoA.value) {
    videoA.value.play();
  }
  // Preload the next video for smoother transition
  const nextIndex = getNextVideoIndex(currentVideoIndex);
  videoSrcB.value = videoFiles[nextIndex];
});
</script>
<style scoped>
.hero-video-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  z-index: 0;
  overflow: hidden;
}
.hero-video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  object-fit: cover;
  border-radius: 0;
  box-shadow: none;
  display: block;
  opacity: 0;
  transition: opacity 0.8s;
  pointer-events: none;
}
.hero-video.visible {
  opacity: 1;
  z-index: 1;
}
.hero-video.hidden {
  opacity: 0;
  z-index: 0;
}
.hero-text {
  position: relative;
  z-index: 2;
  text-align: center;
  margin-top: 48px;
  color: #fff;
  text-shadow: 0 2px 8px rgba(0,0,0,0.5);
}
</style>
