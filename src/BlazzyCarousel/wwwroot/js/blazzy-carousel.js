let swiperLoaded = false;
let swiperInstance = null;

/* -----------------------------
   Utility: Dynamic script loader
----------------------------- */
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

/* -----------------------------
   Utility: Dynamic stylesheet loader
----------------------------- */
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

/* -----------------------------
   Ensure Swiper is available
----------------------------- */
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
    console.log("[BlazzyCarousel] Swiper library ready");
}

/* -----------------------------
   Initialize carousel
----------------------------- */
export async function initializeCarousel(element, optionsJson) {
    try {
        console.log("[BlazzyCarousel] Initializing carousel...");

        const container = element.querySelector(".swiper-container");
        if (!container) {
            console.error("[BlazzyCarousel] .swiper-container NOT FOUND");
            return;
        }


        console.log("[Diag] #1 slides now:", container.querySelectorAll(".swiper-slide").length,
            "containerWidth:", container.clientWidth);


        await new Promise(r => requestAnimationFrame(() => requestAnimationFrame(r)));


        console.log("[Diag] #2 slides after RAFx2:", container.querySelectorAll(".swiper-slide").length,
            "containerWidth:", container.clientWidth);


        const images = Array.from(container.querySelectorAll("img"));
        await Promise.all(images.map(img => img.decode?.().catch(() => { }) || (img.complete ? Promise.resolve() : new Promise(res => img.onload = img.onerror = res))));


        console.log("[Diag] #3 after images loaded:",
            container.querySelectorAll(".swiper-slide").length,
            "containerWidth:", container.clientWidth);

        if (swiperInstance) {
            swiperInstance.destroy(true, true);
            swiperInstance = null;
        }

        const options = optionsJson ? JSON.parse(optionsJson) : {};
        const slides = container.querySelectorAll(".swiper-slide");
        const slideCount = slides.length;


        const enableLoop = slideCount > 3 && (options.loop ?? true);
        console.log("📊 Total slides before init:", container.querySelectorAll('.swiper-slide').length);
        const wrapper = container.querySelector('.swiper-wrapper');


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
                init: () => console.log("[BlazzyCarousel] coverflow re-enabled ✅"),
                setTranslate: function () {
                    this.slides.forEach(slide => {
                        if (parseInt(slide.style.zIndex) < 0) {
                            slide.style.zIndex = 1;
                        }
                    });
                }
            }
        });

        console.log("[BlazzyCarousel] Carousel initialized successfully");
        console.log("[Diag] wrapper width:", container.querySelector(".swiper-wrapper").offsetWidth);
        container.querySelectorAll(".swiper-slide").forEach((s, i) => {
            console.log(`[Diag] slide ${i}:`, s.getBoundingClientRect());
        });
    } catch (err) {
        console.error("[BlazzyCarousel] Initialization error:", err);
    }
}

export function destroyCarousel() {
    if (swiperInstance) {
        swiperInstance.destroy(true, true);
        swiperInstance = null;
        console.log("[BlazzyCarousel] Destroyed");
    }
}
