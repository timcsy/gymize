from distutils.spawn import find_executable
import glob
import os
import subprocess
import sys
from sys import platform
from typing import Optional, List

def get_platform():
    '''
    returns the platform of the operating system : linux, darwin or win32
    '''
    return platform

def validate_environment_path(env_path: str) -> Optional[str]:
    '''
    Strip out executable extensions of the env_path
    :param env_path: The path to the executable
    '''
    env_path = (
        env_path.strip()
        .replace(".app", "")
        .replace(".exe", "")
        .replace(".x86_64", "")
        .replace(".x86", "")
    )
    true_filename = os.path.basename(os.path.normpath(env_path))
    # print(f"The true file name is {true_filename}")

    if not (glob.glob(env_path) or glob.glob(env_path + ".*")):
        return None

    cwd = os.getcwd()
    launch_string = None
    true_filename = os.path.basename(os.path.normpath(env_path))
    if get_platform() == "linux" or get_platform() == "linux2":
        candidates = glob.glob(os.path.join(cwd, env_path) + ".x86_64")
        if len(candidates) == 0:
            candidates = glob.glob(os.path.join(cwd, env_path) + ".x86")
        if len(candidates) == 0:
            candidates = glob.glob(env_path + ".x86_64")
        if len(candidates) == 0:
            candidates = glob.glob(env_path + ".x86")
        if len(candidates) == 0:
            if os.path.isfile(env_path):
                candidates = [env_path]
        if len(candidates) > 0:
            launch_string = candidates[0]

    elif get_platform() == "darwin":
        candidates = glob.glob(
            os.path.join(cwd, env_path + ".app", "Contents", "MacOS", true_filename)
        )
        if len(candidates) == 0:
            candidates = glob.glob(
                os.path.join(env_path + ".app", "Contents", "MacOS", true_filename)
            )
        if len(candidates) == 0:
            candidates = glob.glob(
                os.path.join(cwd, env_path + ".app", "Contents", "MacOS", "*")
            )
        if len(candidates) == 0:
            candidates = glob.glob(
                os.path.join(env_path + ".app", "Contents", "MacOS", "*")
            )
        if len(candidates) > 0:
            launch_string = candidates[0]
    elif get_platform() == "win32":
        candidates = glob.glob(os.path.join(cwd, env_path + ".exe"))
        if len(candidates) == 0:
            candidates = glob.glob(env_path + ".exe")
        if len(candidates) == 0:
            # Look for e.g. 3DBall\UnityEnvironment.exe
            crash_handlers = set(
                glob.glob(os.path.join(cwd, env_path, "UnityCrashHandler*.exe"))
            )
            candidates = [
                c
                for c in glob.glob(os.path.join(cwd, env_path, "*.exe"))
                if c not in crash_handlers
            ]
        if len(candidates) > 0:
            launch_string = candidates[0]
    return launch_string


def launch_executable(file_name: str, args: List[str]) -> subprocess.Popen:
    '''
    Launches a Unity executable and returns the process handle for it.
    :param file_name: the name of the executable
    :param args: List of string that will be passed as command line arguments
    when launching the executable.
    '''
    launch_string = validate_environment_path(file_name)
    if launch_string is None:
        print(f"Couldn't launch the {file_name} environment. Provided filename does not match any environments.")
    else:
        # Launch Unity environment
        subprocess_args = [launch_string] + args
        try:
            return subprocess.Popen(
                subprocess_args,
                # start_new_session=True means that signals to the parent python process
                # (e.g. SIGINT from keyboard interrupt) will not be sent to the new process on POSIX platforms.
                # This is generally good since we want the environment to have a chance to shutdown,
                # but may be undesirable in come cases; if so, we'll add a command-line toggle.
                # Note that on Windows, the CTRL_C signal will still be sent.
                start_new_session=True,
                env=os.environ,
            )
        except PermissionError:
            # This is likely due to missing read or execute permissions on file.
            print(
                'Error when trying to launch environment - make sure '
                'permissions are set correctly. For example '
                f'"chmod -R 755 {launch_string}"'
            )

def launch_env(file_name: str=None, args: List[str]=list()):
    if file_name is not None:
        vglrun: str = find_executable('vglrun') # Check whether vglrun is on PATH
        if vglrun is None:
            print('Run ' + ' '.join([file_name] + args))
            return launch_executable(file_name, args)
        else:
            print('Run ' + ' '.join([vglrun, file_name] + args))
            return launch_executable(vglrun, [file_name] + args)
    else:
        print('Please start the Unity game in the Unity Editor or open the game manually!')


if __name__ == "__main__":
    file_name = None
    if len(sys.argv) > 1:
        file_name = sys.argv[1]
    
    launch_env(file_name=file_name, args=sys.argv[2:])