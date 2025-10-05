cmake_minimum_required (VERSION 3.28)

project ("Sceelix")

set(CPACK_PACKAGE_VERSION 1.2.0)

set(SolutionRoot ${CMAKE_SOURCE_DIR}/Source)
set(SetupDataDir ${CMAKE_SOURCE_DIR}/Setup)
set(GeneratedFiles ${CMAKE_BINARY_DIR}/Generated)

file(MAKE_DIRECTORY ${GeneratedFiles})

message(STATUS "Source dir   : ${CMAKE_SOURCE_DIR}")
message(STATUS "Binary dir   : ${CMAKE_BINARY_DIR}")
message(STATUS "OS           : ${CMAKE_SYSTEM_NAME}")
message(STATUS "Architecture : ${CMAKE_SYSTEM_PROCESSOR}")
message(STATUS "Version      : ${CPACK_PACKAGE_VERSION}")

set(TARGET_SYSTEM Linux)
################## Building ##############################################

add_custom_target(MainExe
    # .sln works with $(Platform), .csproj can do with $(RuntimeIdentifier), so choose before use one.
    COMMAND dotnet publish Sceelix.sln /p:Platform=${TARGET_SYSTEM} /p:Version=${CPACK_PACKAGE_VERSION}
    WORKING_DIRECTORY ${SolutionRoot}
    USES_TERMINAL
)

set(MainExeName Sceelix.Designer.exe)   # net48, uses .exe extension, even in Linux
set(MainExeDir ${CMAKE_SOURCE_DIR}/Build/${TARGET_SYSTEM}/Release/Bin/net48/publish)

################## Package metadata #######################################

set(CPACK_PACKAGE_NAME "sceelix")
set(CMAKE_PROJECT_HOMEPAGE_URL "https://github.com/willneedit(Sceelix)")
set(CPACK_PACKAGE_VENDOR "willneedit")
set(CPACK_PACKAGE_CONTACT "willneedit")
set(CPACK_PACKAGE_DESCRIPTION "Sceelix - Procedural Power")
set(CPACK_PACKAGE_DESCRIPTION_SUMMARY "Sceelix - Procedural Power")
set(CPACK_PACKAGE_INSTALL_DIRECTORY "Sceelix")

#set(CPACK_RESOURCE_FILE_LICENSE ${SetupDataDir}/Windows/SetupLicense.rtf)

if(${TARGET_SYSTEM} STREQUAL "Linux")
    set(CPACK_PACKAGING_INSTALL_PREFIX "/usr/")
	set(CPACK_GENERATOR "DEB")

	set(CPACK_PACKAGE_FILE_NAME ${CPACK_PACKAGE_NAME}-${CPACK_PACKAGE_VERSION})

	set(CPACK_DEBIAN_PACKAGE_SECTION "games")
	# set(CPACK_DEBIAN_PACKAGE_CONTROL_EXTRA ${GeneratedFiles}/postinst;${GeneratedFiles}/prerm)
	set(CPACK_DEBIAN_PACKAGE_DEPENDS
        mono-runtime
	# 	dotnet-runtime-8.0
	)
else()
	message(FATAL_ERROR "Unsupported installer generator for target architecture")
endif()

add_custom_target(Installer
	COMMAND ${CMAKE_MAKE_PROGRAM} "package"
	DEPENDS MainExe
	WORKING_DIRECTORY ${CMAKE_BINARY_DIR}
    USES_TERMINAL
)

include(CPack)
include(GNUInstallDirs)

################## Target paths ##########################################

set(TargetLibexecDir ${CMAKE_INSTALL_LIBEXECDIR}/${CPACK_PACKAGE_NAME})
set(TargetMainExeName ${MainExeName})

if(${TARGET_SYSTEM} STREQUAL "Linux")
    # bin/ should have a single file, or a symlink
    set(TargetMainExeDir ${TargetLibexecDir})
else()
	# Windows keeps these in the Program Files directory
	set(TargetMainExeDir ${CMAKE_INSTALL_BINDIR})
endif()

################## Generated files #######################################

configure_file(
	${SetupDataDir}/Linux/Sceelix.in
	${GeneratedFiles}/Sceelix
	FILE_PERMISSIONS OWNER_READ OWNER_WRITE OWNER_EXECUTE GROUP_READ GROUP_EXECUTE WORLD_READ WORLD_EXECUTE
	@ONLY
	NEWLINE_STYLE LF
)



################## Packaging #############################################

install(DIRECTORY
    ${MainExeDir}/
	FILE_PERMISSIONS OWNER_READ OWNER_WRITE OWNER_EXECUTE GROUP_READ GROUP_EXECUTE WORLD_READ WORLD_EXECUTE
	DESTINATION ${TargetMainExeDir}
)

if(${TARGET_SYSTEM} STREQUAL "Linux")
    # Linux: Program package resides in /libexec/<package>, set a link in /bin
    install(PROGRAMS
        ${GeneratedFiles}/Sceelix
        DESTINATION ${CMAKE_INSTALL_BINDIR}
    )
endif()
