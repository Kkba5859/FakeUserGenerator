self.importScripts('./service-worker-assets.js');

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html$/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

self.addEventListener('install', event => event.waitUntil(onInstall()));
self.addEventListener('activate', event => event.waitUntil(onActivate()));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

async function onInstall() {
    const cache = await openCache();
    await cacheAssets(cache);
}

async function onActivate() {
    const cacheKeys = await caches.keys();
    await removeOldCaches(cacheKeys);
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);

    const cache = await openCache();

    if (isNavigateRequest(event)) {
        return cache.match('index.html') || fetch(event.request);
    }

    if (isCachedAssetRequest(event)) {
        return cache.match(event.request) || fetch(event.request);
    }

    if (isUserDataRequest(event)) {
        return handleUserDataRequest(event, cache);
    }

    return fetch(event.request);
}

async function openCache() {
    return await caches.open(cacheName);
}

async function cacheAssets(cache) {
    const assetRequests = self.assetsManifest.assets
        .filter(asset => shouldCacheAsset(asset.url))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    return cache.addAll(assetRequests);
}

function shouldCacheAsset(url) {
    return offlineAssetsInclude.some(pattern => pattern.test(url)) &&
        !offlineAssetsExclude.some(pattern => pattern.test(url));
}

async function removeOldCaches(cacheKeys) {
    const oldCacheKeys = cacheKeys.filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName);
    return Promise.all(oldCacheKeys.map(key => caches.delete(key)));
}

function isNavigateRequest(event) {
    return event.request.mode === 'navigate' && !manifestUrlList.includes(event.request.url);
}

function isCachedAssetRequest(event) {
    return offlineAssetsInclude.some(pattern => pattern.test(event.request.url));
}

function isUserDataRequest(event) {
    return event.request.url.includes('/api/userdata'); 
}

async function handleUserDataRequest(event, cache) {
    try {
        const response = await fetch(event.request);
        if (response.ok) {
            const clonedResponse = response.clone();
            cache.put(event.request, clonedResponse);
            return response;
        }
    } catch (error) {
        console.error('Error fetching data:', error);
    }

    return cache.match(event.request) || fetch(event.request);
}

window.saveAsFile = function (filename, byteBase64) {
    let link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
