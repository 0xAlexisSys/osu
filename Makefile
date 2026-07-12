NAME := osu-sp
VERSION ?= 1.0.0
ICON_EXTENSION := .png
ICON_FILE := Assets/Icon$(ICON_EXTENSION)
DIST_DIR := .dist

.PHONY: all windows windows-pkg linux linux-pkg ios ios-pkg android android-pkg clean

all: windows linux

WINDOWS_DIST_DIR = $(DIST_DIR)/Windows
WINDOWS_OUTPUT_FILE = $(WINDOWS_DIST_DIR)/$(NAME)-$(VERSION)-windows-x86_64.zip

windows:
	dotnet publish "osu.Desktop.slnf" --configuration Release --runtime win-x64 --self-contained -p:Version="$(VERSION)"

# [alexis] The dot and asterisk are needed at the start and end of the source path so
#          7-Zip doesn't preserve the leading directories.
windows-pkg:
	rm -f "$(WINDOWS_OUTPUT_FILE)"

	mkdir -p "$(WINDOWS_DIST_DIR)/"
	7z a -mx=9 -mmt=$(nproc) "$(WINDOWS_OUTPUT_FILE)" "./osu.Desktop/.build/bin/Release/win-x64/publish/*"

LINUX_DIST_DIR = $(DIST_DIR)/Linux
LINUX_APPIMAGE_BIN_INTERNAL_DIR = /usr/bin
LINUX_APPIMAGE_EXEC_INTERNAL_FILE = $(LINUX_APPIMAGE_BIN_INTERNAL_DIR)/osu.Desktop
LINUX_APPIMAGE_DIR = $(LINUX_DIST_DIR)/AppDir
LINUX_APPIMAGE_BIN_DIR = $(LINUX_APPIMAGE_DIR)$(LINUX_APPIMAGE_BIN_INTERNAL_DIR)
LINUX_APPIMAGE_ICON_DIR = $(LINUX_APPIMAGE_DIR)/usr/share/icons/hicolor/256x256/apps
LINUX_APPIMAGE_APPRUN_FILE = $(LINUX_APPIMAGE_DIR)/AppRun
LINUX_OUTPUT_FILE = $(LINUX_DIST_DIR)/$(NAME)-$(VERSION)-linux-x86_64.AppImage

linux:
	dotnet publish "osu.Desktop.slnf" --configuration Release --runtime linux-x64 --self-contained -p:Version="$(VERSION)"

linux-pkg:
	rm -rf "$(LINUX_APPIMAGE_DIR)"
	rm -f "$(LINUX_OUTPUT_FILE)"

	mkdir -p "$(LINUX_APPIMAGE_BIN_DIR)/"
	cp -r "osu.Desktop/.build/bin/Release/linux-x64/publish/." "$(LINUX_APPIMAGE_BIN_DIR)/"

	mkdir -p "$(LINUX_APPIMAGE_ICON_DIR)/"
	cp "$(ICON_FILE)" "$(LINUX_APPIMAGE_DIR)/$(NAME)$(ICON_EXTENSION)"
	cp "$(ICON_FILE)" "$(LINUX_APPIMAGE_DIR)/.DirIcon"
	cp "$(ICON_FILE)" "$(LINUX_APPIMAGE_ICON_DIR)/$(NAME)$(ICON_EXTENSION)"

	@printf '%s\n' \
	'[Desktop Entry]' \
	'Type=Application' \
	'Name=$(NAME)' \
	'Comment=A single-player fork of osu!' \
	'Exec="$(LINUX_APPIMAGE_EXEC_INTERNAL_FILE)" %U' \
	'Icon=$(NAME)' \
	'Categories=Game' \
	'Terminal=false' \
	'StartupWMClass=$(NAME)' \
	> "$(LINUX_APPIMAGE_DIR)/$(NAME).desktop"

	@printf '%s\n' \
	'#!/usr/bin/env bash' \
	'' \
	'HERE="$$(dirname "$$(readlink -f "$${0}")")"' \
	'' \
	'export PATH="$${HERE}$(LINUX_APPIMAGE_BIN_INTERNAL_DIR):$${PATH}"' \
	'export LD_LIBRARY_PATH="$${HERE}/usr/lib:$${LD_LIBRARY_PATH}"' \
	'' \
	'exec "$${HERE}$(LINUX_APPIMAGE_EXEC_INTERNAL_FILE)" "$$@"' \
	> "$(LINUX_APPIMAGE_APPRUN_FILE)"
	chmod +x "$(LINUX_APPIMAGE_APPRUN_FILE)"

	ARCH=x86_64 appimagetool --no-appstream "$(LINUX_APPIMAGE_DIR)" "$(LINUX_OUTPUT_FILE)"

ios:
	echo "ios is not implemented"

ios-pkg:
	echo "ios-pkg is not implemented"

ANDROID_RUNTIME ?= ARM64

android:
ifeq ("$(ANDROID_RUNTIME)", "X86_64")
	dotnet publish "osu.Android.slnf" --configuration Release --runtime android-x64 -p:Version="$(VERSION)" -p:AndroidPackageFormat=apk
else ifeq ("$(ANDROID_RUNTIME)", "ARM64")
	dotnet publish "osu.Android.slnf" --configuration Release --runtime android-arm64 -p:Version="$(VERSION)" -p:AndroidPackageFormat=apk
else ifeq ("$(ANDROID_RUNTIME)", "ALL")
	dotnet publish "osu.Android.slnf" --configuration Release --runtime android-x64 -p:Version="$(VERSION)" -p:AndroidPackageFormat=apk
	dotnet publish "osu.Android.slnf" --configuration Release --runtime android-arm64 -p:Version="$(VERSION)" -p:AndroidPackageFormat=apk
else
	@echo "Specified ANDROID_RUNTIME is invalid."
	@echo "Valid options:\n"
	@echo "1. X86_64 (android-x64)"
	@echo "2. ARM64 (android-arm64)"
	@echo "3. ALL (android-x64, android-arm64)"
endif

clean:
	rm -rf "**/.build/"
	rm -rf "**/bin/"
	rm -rf "**/obj/"
