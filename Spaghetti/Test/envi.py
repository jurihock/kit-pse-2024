import numpy as np
import os
import spectral
import spectral.io.envi

spectral.settings.envi_support_nonlowercase_params = True

def load_envi_image(path, *, interleave='bip', writable=False):
    """
    Reads image data from the specified ENVI image file.

    Parameters
    ----------
    path : string
        ENVI image file path, e.g. .img or .raw (not .hdr)
    interleave : string, optional
        Memory mapping layout bip, bil or bsq
        (defaults to bip, e.g. lines, samples, bands)
    writable : bool, optional
        Allow image data modifications
        (defaults to False)

    Returns
    -------
    image : numpy.memmap
        Image data as numpy ndarray
    """

    path, ext = os.path.splitext(path)

    image = spectral.io.envi.open(f'{path}.hdr', f'{path}{ext}')

    return image.open_memmap(interleave=interleave, writable=writable)

def save_envi_image(image, path, *, meta={}, interleave='bip'):
    """
    Writes image data to the specified ENVI image file.

    Parameters
    ----------
    image : ndarray
        Image data to be saved
    path : string
        ENVI image file path, e.g. .img or .raw (not .hdr)
    meta : dict, optional
        Additional ENVI header parameters (key + value)
    interleave : string, optional
        Memory mapping layout bip, bil or bsq
        (defaults to bip, e.g. lines, samples, bands)
    """

    image = np.atleast_3d(image)

    assert image.ndim == 3, f'Invalid number of image data array dimensions {image.ndim}!'

    path, ext = os.path.splitext(path)

    spectral.io.envi.save_image(
        f'{path}.hdr',
        ext=ext,
        image=image,
        dtype=image.dtype,
        metadata=meta,
        interleave=interleave,
        force=True)
