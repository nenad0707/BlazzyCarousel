let swiperLoaded = false;
let swiperInstance = null;

/* ═══════════════════════════════════════════════════════════
   Utility: Dynamic script loader
   ═══════════════════════════════════════════════════════════ */
function loadScript(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[src="${src}"]`);
        if (existing) {
            if (typeof Swiper !== "undefined") {
                resolve();
                return;
            }
            const check = setInterval(() => {
                if (typeof Swiper !== "undefined") {
                    clearInterval(check);
                    resolve();
                }
            }, 100);
            return;
        }

        const script = document.createElement("script");
        script.src = src;
        script.async = false;
        script.onload = () => {
            let tries = 0;
            const check = setInterval(() => {
                tries++;
                if (typeof Swiper !== "undefined") {
                    clearInterval(check);
                    resolve();
                } else if (tries > 50) {
                    clearInterval(check);
                    reject(new Error("Swiper script loaded but not defined"));
                }
            }, 100);
        };
        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
        document.head.appendChild(script);
    });
}

/* ═══════════════════════════════════════════════════════════
   Utility: Dynamic stylesheet loader
   ═══════════════════════════════════════════════════════════ */
function loadStylesheet(href) {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`link[href="${href}"]`)) {
            resolve();
            return;
        }
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.onload = () => resolve();
        link.onerror = () => reject(new Error(`Failed to load stylesheet: ${href}`));
        document.head.appendChild(link);
    });
}

/* ═══════════════════════════════════════════════════════════
   Ensure Swiper is available
   ═══════════════════════════════════════════════════════════ */
export async function ensureSwiperLoaded() {
    if (swiperLoaded) return;

    await Promise.all([
        loadStylesheet("_content/BlazzyCarousel/css/swiper-bundle.min.css"),
        loadStylesheet("_content/BlazzyCarousel/css/blazzy-carousel.css"),
    ]);
    await loadScript("_content/BlazzyCarousel/js/swiper-bundle.min.js");

    if (typeof Swiper === "undefined") {
        throw new Error("Swiper is not defined after load");
    }

    swiperLoaded = true;
    console.log("[BlazzyCarousel] Swiper library ready ✅");
}

/* ═══════════════════════════════════════════════════════════
   Initialize carousel
   ═══════════════════════════════════════════════════════════ */
export async function initializeCarousel(element, optionsJson) {
    try {
        const container = element.querySelector(".swiper-container");
        if (!container) {
            console.error("[BlazzyCarousel] .swiper-container NOT FOUND");
            return;
        }

        // ✅ Čekaj da se DOM stabiliše
        await new Promise(r => requestAnimationFrame(() => requestAnimationFrame(r)));

        // ✅ Uništi stari instance ako postoji
        if (swiperInstance) {
            swiperInstance.destroy(true, true);
            swiperInstance = null;
        }

        const options = optionsJson ? JSON.parse(optionsJson) : {};

        // ✅ Inicijalizuj Swiper
        swiperInstance = new Swiper(container, {
            effect: "coverflow",
            grabCursor: true,
            centeredSlides: true,
            slidesPerView: "auto",
            initialSlide: options.initialSlide || 0,
            loop: options.loop ?? true,
            speed: 300,
            slideToClickedSlide: true,
            watchSlidesProgress: true,
            watchSlidesVisibility: true,
            observer: true,
            observeParents: true,
            coverflowEffect: {
                rotate: 50,
                stretch: 0,
                depth: 150,
                modifier: 1.2,
                slideShadows: true,
            },
            on: {
                init: () => console.log("[BlazzyCarousel] Carousel initialized ✅"),
                setTranslate: function () {
                    this.slides.forEach(slide => {
                        if (parseInt(slide.style.zIndex) < 0) {
                            slide.style.zIndex = 1;
                        }
                    });
                }
            }
        });

    } catch (err) {
        console.error("[BlazzyCarousel] Initialization error:", err);
    }
}

/* ═══════════════════════════════════════════════════════════
   Destroy carousel
   ═══════════════════════════════════════════════════════════ */
export function destroyCarousel() {
    if (swiperInstance) {
        swiperInstance.destroy(true, true);
        swiperInstance = null;
        console.log("[BlazzyCarousel] Destroyed ✅");
    }
}