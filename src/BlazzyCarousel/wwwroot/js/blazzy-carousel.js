let swiperLoaded = false;
const swiperInstances = new Map();

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
            }

                , 100);
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
                }

                else if (tries > 50) {
                    clearInterval(check);
                    reject(new Error("Swiper script loaded but not defined"));
                }
            }

                , 100);
        }

            ;

        script.onerror = () => reject(new Error(`Failed to load script: $ {
                        src
                    }

                    `));
        document.head.appendChild(script);
    }

    );
}

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

        link.onerror = () => reject(new Error(`Failed to load stylesheet: $ {
                        href
                    }

                    `));
        document.head.appendChild(link);
    }

    );
}

export async function ensureSwiperLoaded() {
    if (swiperLoaded) return;

    await Promise.all([loadStylesheet("_content/BlazzyCarousel/css/swiper-bundle.min.css"),
    loadStylesheet("_content/BlazzyCarousel/css/blazzy-carousel.css"),
    ]);
    await loadScript("_content/BlazzyCarousel/js/swiper-bundle.min.js");

    if (typeof Swiper === "undefined") {
        throw new Error("Swiper is not defined after load");
    }

    swiperLoaded = true;
}

export async function initializeCarousel(element, optionsJson) {
    try {
        const container = element.querySelector(".swiper-container");

        if (!container) {
            return;
        }

        if (swiperInstances.has(element)) {
            const instance = swiperInstances.get(element);
            instance.destroy(true, true);
            swiperInstances.delete(element);
        }

        const options = optionsJson ? JSON.parse(optionsJson) : {};

        const swiperInstance = new Swiper(container, {
            effect: options.effect || "coverflow",
            grabCursor: options.grabCursor ?? true,
            centeredSlides: options.centeredSlides ?? true,
            slidesPerView: options.slidesPerView || "auto",
            initialSlide: options.initialSlide || 0,
            loop: options.loop ?? true,
            loopAdditionalSlides: 2,
            speed: 0,
            runCallbacksOnInit: false,
            slideToClickedSlide: true,
            watchSlidesProgress: true,
            watchSlidesVisibility: true,
            observer: true,
            observeParents: true,
            touchRatio: 1,
            touchEventsTarget: 'container',
            coverflowEffect: {
                rotate: options.rotateDegree || 50,
                stretch: options.stretch || 0,
                depth: options.depth || 150,
                modifier: options.modifier || 1.5,
                slideShadows: options.slideShadows ?? true,
            },
            on: {
                setTranslate: function () {
                    this.slides.forEach(slide => {
                        const currentZ = parseInt(slide.style.zIndex);

                        if (currentZ < 0 || isNaN(currentZ)) {
                            slide.style.zIndex = '1';
                        }

                        slide.style.pointerEvents = 'auto';
                    });

                    if (this.params.speed === 0) {
                        this.params.speed = 300;
                        this.params.runCallbacksOnInit = true;

                        setTimeout(() => {
                            this.el.classList.remove('bzc-hidden');
                            this.el.classList.add('bzc-visible');
                        }, 100);
                    }
                }
            }
        });

        swiperInstances.set(element, swiperInstance);
    }
    catch (err) {
        console.error("[BlazzyCarousel] Initialization error:", err);
    }
}

export function destroyCarousel(element) {
    if (swiperInstances.has(element)) {
        const instance = swiperInstances.get(element);
        instance.destroy(true, true);
        swiperInstances.delete(element);
    }
}

export function getActiveIndex(element) {
    if (swiperInstances.has(element)) {
        return swiperInstances.get(element).activeIndex;
    }
    return 0;
}