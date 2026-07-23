### Example of recursive infinite response filter (IIR low pass, first order)

Example of how to use the VCH programming language 

// Example of recursive infinite response filter (IIR low pass, first order)

// Variable Declaration

dim val_filt;

dim pre_val_filt;

dim val_raw;

// "cut_freq" is the Cutoff frequency of filter

dim cut_freq;

cut_freq = 0.1;

// Filter coefficients declaration

dim a0;

dim b1;

dim X;

// Filter coefficients calculation

val_raw = val_filt = Laterl_Acceleration;

X = 1 / (1 + 2 * pi * cut_freq/chfreq(Laterl_Acceleration));

a0 = 1-X;

b1 = X;

if( !isnan(val_raw) && !isnan(pre_val_filt) )

{ val_filt = a0* val_raw + b1 * pre_val_filt; }

pre_val_filt = val_filt;

return val_filt;